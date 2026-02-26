using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    public InputActionReference interactAction; // E Tuşu (sadece interact/exam)
    public InputActionReference holdAction;     // Sol Tık (sadece pickup)

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    [Header("Physics Settings (Pickup için)")]
    public float followSpeed = 40f;
    public float dragAmount = 15f;
    public float rotationSpeed = 20f;

    private GameObject heldObject;
    private Rigidbody heldRigidbody;
    private float currentHoldDistance;
    private Quaternion initialRotationOffset;

    public float objectRotationSpeed = 100f;
    private float currentRotationInput;
    private IReadable currentReadable;
    
    public void SetRotationInput(float input)
    {
        currentRotationInput = input;
    }
    private void OnEnable()
    {
        interactAction.action.performed += OnInteractTriggered;
        holdAction.action.performed += OnHoldStarted;
        holdAction.action.canceled += OnHoldCanceled;

        interactAction.action.Enable();
        holdAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractTriggered;
        holdAction.action.performed -= OnHoldStarted;
        holdAction.action.canceled -= OnHoldCanceled;

        interactAction.action.Disable();
        holdAction.action.Disable();
    }

    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            MoveHeldObject();
            HandleObjectRotation();
        }
            
    }

    private void HandleObjectRotation()
    {
        if (Mathf.Abs(currentRotationInput) > 0.01f)
        {
            float rotationAmount = currentRotationInput * objectRotationSpeed * Time.fixedDeltaTime;
            heldObject.transform.Rotate(Vector3.up, -rotationAmount, Space.World);

            // So MoveHeldObject doesn't Slerp back: make the "target" include this rotation
            initialRotationOffset = Quaternion.Inverse(mainCamera.transform.rotation) * heldObject.transform.rotation;
        }
    }
    private void Update()
    {
        if (currentReadable == null)
        {
            Debug.Log("currentreadable null!");
        }
        ClearDestroyedReadable();

        if (currentReadable != null)
        {
            if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame)
                currentReadable.NextPage();
            else if (Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame)
                currentReadable.PreviousPage();
        }
    }

    private void ClearDestroyedReadable()
    {
        if (currentReadable is MonoBehaviour mb && mb == null)
            currentReadable = null;
    }

    private void OnInteractTriggered(InputAction.CallbackContext context)
    {
        ClearDestroyedReadable();

        if (currentReadable != null)
        {
            currentReadable.Close();
            currentReadable = null;
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance, interactableLayer))
        {
            IReadable readable = hit.collider.GetComponentInParent<IReadable>();

            if (readable != null)
            {
                currentReadable = readable; // memorize
                currentReadable.Open();
                return;
            }

        }
    }

    // SOL TIK BAŞLANGIÇ: PICKUP AMA BOOK SKIP
    private void OnHoldStarted(InputAction.CallbackContext context)
    {
        if (currentReadable != null) return;
        if (heldObject == null) HandleInteraction();
    }

    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        if (heldObject != null) DropObject();
    }

    // HOLD İÇİ ETKİLEŞİM: BOOK VARSA SKIP (PICKUP ETME)
    private void HandleInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance, interactableLayer))
        {
            // ÖNCE BOOK KONTROL: Varsa pickup'ı SKIP et (statik tut)
            if (hit.collider.GetComponentInParent<IReadable>() != null)
            {
                return;  // Kitap pickup olmaz!
            }

            // NORMAL INTERACTABLE
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.OnInteract();

            // PICKUP
            IPickable pickable = hit.collider.GetComponent<IPickable>();
            if (pickable != null)
            {
                currentHoldDistance = hit.distance;
                initialRotationOffset = Quaternion.Inverse(mainCamera.transform.rotation) * hit.collider.transform.rotation;
                PickupObject(hit.collider.gameObject);
                pickable.OnPickedUp();
            }
        }
    }

    // Pickup metodları aynı kalır...
    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldRigidbody = obj.GetComponent<Rigidbody>();

        heldRigidbody.isKinematic = false;
        heldRigidbody.useGravity = false;
        heldRigidbody.linearDamping = dragAmount;
        heldRigidbody.angularDamping = dragAmount;
        heldRigidbody.linearVelocity = Vector3.zero;
        heldRigidbody.angularVelocity = Vector3.zero;
        heldRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        heldObject.transform.SetParent(null);
    }

    private void DropObject()
    {
        IPickable pickable = heldObject.GetComponent<IPickable>();
        pickable?.OnDropped();

        heldRigidbody.isKinematic = false;
        heldRigidbody.useGravity = true;
        heldRigidbody.linearDamping = 0f;
        heldRigidbody.angularDamping = 0.05f;
        heldRigidbody.AddForce(mainCamera.transform.forward * 1.5f, ForceMode.Impulse);

        heldObject = null;
        heldRigidbody = null;
    }

    private void MoveHeldObject()
    {
        Vector3 targetPosition = mainCamera.transform.position + (mainCamera.transform.forward * currentHoldDistance);
        Vector3 distanceDelta = targetPosition - heldObject.transform.position;
        heldRigidbody.linearVelocity = distanceDelta * followSpeed;

        Quaternion targetRotation = mainCamera.transform.rotation * initialRotationOffset;
        heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }
}