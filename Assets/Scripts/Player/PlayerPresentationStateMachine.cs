using UnityEngine;
using System.Collections;

public class PlayerPresentationStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera mainCamera;

    [Header("Desk Focus")]
    [SerializeField] private Transform deskFocusTarget;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0,0,1,1);
    
    public PlayerPresentationState CurrentState { get; private set; }
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }

        if( mainCamera == null )
        {
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        EnterDeskFocusImmediate();
    }

    public void EnterDeskFocusImmediate()
    {
        StopActiveTransition();
        CurrentState = PlayerPresentationState.DeskFocus;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(true);

            if (deskFocusTarget != null)
            {
                playerController.SnapToTransform(deskFocusTarget);
            }
        }

        SetCursorLocked();
    }

    public void EnterDeskFocus()
    {
        StopActiveTransition();

        if (deskFocusTarget == null || playerController == null)
        {
            EnterDeskFocusImmediate();
            return;
        }

        transitionCoroutine = StartCoroutine(TransitionToDeskFocusCoroutine());
    }

    public void EnterUIInteraction()
    {
        StopActiveTransition();

        CurrentState = PlayerPresentationState.UIInteraction;

        if(playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(false);
        }
        SetCursorUnlocked();
    }

    public void EnterBookExamination(Transform bookCameraTarget)
    {
        CurrentState = PlayerPresentationState.BookExamination;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(false);
        }

        if (mainCamera != null && bookCameraTarget != null)
        {
            mainCamera.transform.position = bookCameraTarget.position;
            mainCamera.transform.rotation = bookCameraTarget.rotation;
        }

        SetCursorUnlocked();
    }

    public void ExitBookExamination()
    {
        EnterDeskFocus();
    }

    public void EnterDocumentExamination()
    {
        StopActiveTransition();

        CurrentState = PlayerPresentationState.DocumentExamination;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(true);
            playerController.SetDocumentLookLimitsEnabled(true);
        }

        SetCursorLocked();
    }

    public void ExitDocumentExamination()
    {
        if (playerController != null)
            playerController.SetDocumentLookLimitsEnabled(false);
        EnterDeskFocus();
    }

    private IEnumerator TransitionToDeskFocusCoroutine()
    {
        EnterDeskFocusImmediate();
        yield break;
    }

    private IEnumerator TransitionToTargetStateCoroutine(PlayerPresentationState targetState, Transform target, bool unlockCursor)
    {
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(false );
        }

        Transform cameraTransform = mainCamera.transform;

        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;

        Vector3 endPosition = target.position;
        Quaternion endRotation = target.rotation;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curvedT = transitionCurve.Evaluate(t);

            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, curvedT);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, endRotation, curvedT);

            yield return null;
        }

        cameraTransform.position = endPosition;
        cameraTransform.rotation = endRotation;

        CurrentState = targetState;

        if (unlockCursor)
            SetCursorUnlocked();
        else
            SetCursorLocked();

        transitionCoroutine = null;
    }

    private void StopActiveTransition()
    {
        if(transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine); 
            transitionCoroutine = null;
        }
    }

    private void SetCursorLocked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetCursorUnlocked()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

    }
}
