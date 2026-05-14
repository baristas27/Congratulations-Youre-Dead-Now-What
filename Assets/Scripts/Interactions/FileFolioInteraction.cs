using System.Collections;
using UnityEngine;

public class FileFolioInteraction : MonoBehaviour, IReadable
{
    [Header("References")]
    [SerializeField] private PhysicalCaseFileView caseFileView;
    [SerializeField] private PlayerPresentationStateMachine presentationStateMachine;
    [SerializeField] private Transform fileHoldPoint;
    private Transform originalParent;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.35f;

    [SerializeField] private GameObject reviewIndicator;

    private bool isOpen;
    private bool isAvailableForReview = true;

    private Vector3 deskPosition;
    private Quaternion deskRotation;
    private Coroutine moveRoutine;

    public System.Action OnReviewFinished;
    private bool hasBeenOpenedForReview;

    private void Awake()
    {
        deskPosition = transform.position;
        deskRotation = transform.rotation;
        originalParent = transform.parent;


        if (caseFileView != null)
            caseFileView.SetPromptVisible(false);
    }

    public void Open()
    {

        if (reviewIndicator != null)
        {
            reviewIndicator.SetActive(false);
        }


        if (!isAvailableForReview)
            return;

        hasBeenOpenedForReview = true;

        if (isOpen)
            return;

        if (fileHoldPoint == null)
        {
            Debug.LogWarning($"{name}: FileHoldPoint is not assigned.");
            return;
        }

        isOpen = true;

        if (presentationStateMachine != null)
            presentationStateMachine.EnterDocumentExamination();

        if (caseFileView != null)
        {
            caseFileView.ShowPage(0);
            caseFileView.SetPromptVisible(true);
        }

        StartMove(fileHoldPoint.position, fileHoldPoint.rotation);
    }

    public void Close()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (caseFileView != null)
            caseFileView.SetPromptVisible(false);

        transform.SetParent(originalParent);

        StartMove(deskPosition, deskRotation);

        if (presentationStateMachine != null)
            presentationStateMachine.ExitDocumentExamination();

        if (hasBeenOpenedForReview)
        {
            hasBeenOpenedForReview = false;
            SetAvailableForReview(false);
            OnReviewFinished?.Invoke();
        }
    }

    public void NextPage()
    {
        if (!isOpen)
            return;

        if (caseFileView != null)
            caseFileView.ShowNextPage();
    }

    public void PreviousPage()
    {
        if (!isOpen)
            return;

        if (caseFileView != null)
            caseFileView.ShowPreviousPage();
    }

    public void SetAvailableForReview(bool available)
    {
        isAvailableForReview = available;

        if (reviewIndicator != null)
        {
            reviewIndicator.SetActive(available);
        }
    }

    private void StartMove(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToRoutine(targetPosition, targetRotation));
    }

    private IEnumerator MoveToRoutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        if (isOpen && fileHoldPoint != null)
        {
            transform.SetParent(fileHoldPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        moveRoutine = null;
    }
}