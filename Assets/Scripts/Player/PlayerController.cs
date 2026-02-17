using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;
    public bool canMove = true;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public float upperLookLimit = -80f;
    public float lowerLookLimit = 80f;

    private CharacterController characterController;
    public Camera playerCamera;
    private Vector3 moveDirection;
    private float verticalRotation = 0f;

    private void Awake()
    {
        // component caching, getcomponent çaðrýsýný awake içinde yapýyoruz
        characterController = GetComponent<CharacterController>();
        //playerCamera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!canMove) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Yeni Sistemde tuþ kontrolü
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y = 1;
        if (Keyboard.current.sKey.isPressed) input.y = -1;
        if (Keyboard.current.aKey.isPressed) input.x = -1;
        if (Keyboard.current.dKey.isPressed) input.x = 1;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        moveDirection.x = (forward.x * input.y) + (right.x * input.x);
        moveDirection.z = (forward.z * input.y) + (right.z * input.x);

        // Yerçekimi ve hareket ayný kalýyor...
        if (!characterController.isGrounded) moveDirection.y += gravity * Time.deltaTime;
        characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Fare hareketini Yeni Sistemden okuma
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity * 0.1f);

        verticalRotation -= mouseDelta.y * mouseSensitivity * 0.1f;
        verticalRotation = Mathf.Clamp(verticalRotation, upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }



}
