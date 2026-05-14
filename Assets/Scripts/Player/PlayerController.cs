using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private Camera playerCamera;
    private CharacterController characterController;

    // INPUT ACTIONS
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference rotateAction;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;
    private bool canMove = true;
    private bool canLook = true;

    public float mouseSensitivity = 2f;
    public float upperLookLimit = -80f;
    public float lowerLookLimit = 80f;


    [Header("Document Look Settings")]
    [SerializeField] private bool useDocumentLookLimits;
    [SerializeField] private float documentUpperLookLimit = -12f;
    [SerializeField] private float documentLowerLookLimit = 12f;


    // --- STATE ---
    private Vector2 moveInput;      // WASD input
    private Vector2 lookInput;      // Mouse delta
    private Vector3 moveDirection;  // Computed movement direction
    private float verticalRotation = 0f; // Camera pitch (up/down)


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(canMove)
        {
            HandleMovementWithInput();
        }

        if(canLook)
        {
            HandleRotation();
        }
    }

    private void LateUpdate()
    {
        lookInput = Vector2.zero;
    }
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            moveInput = Vector2.zero;
            moveDirection = Vector3.zero;
        }
    }

    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;

        if (!enabled)
        {
            lookInput = Vector2.zero;
        }
    }

    public void SetDocumentLookLimitsEnabled(bool enabled)
    {
        useDocumentLookLimits = enabled;

        if (enabled)
        {
            verticalRotation = Mathf.Clamp(verticalRotation, documentUpperLookLimit, documentLowerLookLimit);

            if (playerCamera != null)
                playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
    public void SnapToTransform(Transform target)
    {
        if (target == null) return;

        if(characterController != null) 
            characterController.enabled = false;    


        transform.SetPositionAndRotation(target.position, target.rotation);

        verticalRotation = 0f;

        if(playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.identity;

        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
        moveDirection.x = 0f;   

        if(characterController !=null)  
            characterController.enabled=true;




    }
    public void FaceTargetInstant(Transform target)
    {
        if (target == null || playerCamera == null) return;

        Vector3 direction = target.position - playerCamera.transform.position;

        if (direction.sqrMagnitude <= 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        Vector3 euler = lookRotation.eulerAngles;

        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

        float pitch = euler.x;
        if (pitch > 180f) pitch -= 360f;

        verticalRotation = Mathf.Clamp(pitch, upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    private void HandleMovementWithInput()
    {
        // Read current value directly so movement works even if callbacks fire late
        Vector2 move = (moveAction != null && moveAction.action.enabled)
            ? moveAction.action.ReadValue<Vector2>()
            : moveInput;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        moveDirection.x = (forward.x * move.y) + (right.x * move.x);
        moveDirection.z = (forward.z * move.y) + (right.z * move.x);

        if (!characterController.isGrounded)
            moveDirection.y += gravity * Time.deltaTime;
        else
            moveDirection.y = 0f;

        characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (playerCamera == null) return;

        // Read current value directly so look stays in sync each frame regardless of callback order
        Vector2 look = (lookAction != null && lookAction.action.enabled)
            ? lookAction.action.ReadValue<Vector2>()
            : lookInput;

        transform.Rotate(Vector3.up * look.x * mouseSensitivity * 0.1f);

        verticalRotation -= look.y * mouseSensitivity * 0.1f;

        float activeUpperLimit = useDocumentLookLimits ? documentUpperLookLimit : upperLookLimit;
        float activeLowerLimit = useDocumentLookLimits ? documentLowerLookLimit : lowerLookLimit;

        verticalRotation = Mathf.Clamp(verticalRotation, activeUpperLimit, activeLowerLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void OnEnable()
    {
        if (moveAction == null) Debug.LogWarning("PlayerController: moveAction not assigned. Assign Move action in Inspector.", this);
        if (lookAction == null) Debug.LogWarning("PlayerController: lookAction not assigned. Assign Look action in Inspector.", this);

        // 1. Move subscription
        if (moveAction != null)
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMoveCanceled;
            moveAction.action.Enable();
        }

        // 2. Look subscription
        if (lookAction != null)
        {
            lookAction.action.performed += OnLook;
            lookAction.action.Enable();
        }


        // 3. Object rotation subscription (held item rotate)
        if (rotateAction != null)
        {
            rotateAction.action.performed += OnRotate;
            rotateAction.action.canceled += OnRotateCanceled;
            rotateAction.action.Enable();
        }


    }

    private void OnDisable()
    {
        // Unsubscribe to avoid errors when exiting play mode
        if (moveAction != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMoveCanceled;
            moveAction.action.Disable();
        }

        if (lookAction != null)
        {
            lookAction.action.performed -= OnLook;
            lookAction.action.Disable();
        }

        if (rotateAction != null)
        {
            rotateAction.action.performed -= OnRotate;
            rotateAction.action.canceled -= OnRotateCanceled;
            rotateAction.action.Disable();
        }
    }

    #region Input Callbacks

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Key released, clear input
        moveInput = Vector2.zero;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // Held object rotation (A/D or axis)
    private void OnRotate(InputAction.CallbackContext context)
    {
        // 1D axis: -1 or 1
        float rotateValue = context.ReadValue<float>();

        // Forward to InteractionManager for held object rotation
        if (interactionManager != null)
        {
            interactionManager.SetRotationInput(rotateValue);
        }
    }

    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        // Key released, stop rotation
        if (interactionManager != null)
        {
            interactionManager.SetRotationInput(0f);
        }
    }

    #endregion
}

