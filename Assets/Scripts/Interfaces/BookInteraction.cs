using System.Collections;
using UnityEngine;

public interface IBookInteractable { void ToggleExamination(); }

public class BookInteraction : MonoBehaviour, IReadable
{

    [SerializeField] private Animator bookAnimator;
    [Header("Camera Settings")]
    public GameObject examinationCamera;
    public Camera mainCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Transition Settings")]
    public float transitionDuration = 0.75f;
    public AnimationCurve transitionCurve;

    private bool isExamining = false;
    private PlayerController player;

    private int totalPages = 2;
    private int currentPage = 0;
    private bool hasOpenedCover;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        mainCamera = Camera.main;
        if (examinationCamera) examinationCamera.SetActive(false);

        if (transitionCurve == null || transitionCurve.keys.Length == 0)
            transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public void Open()
    {
        if (isExamining) return;

        isExamining = true;
        hasOpenedCover = false;

        StartCoroutine(EnterExaminationCoroutine());
    }

    public void Close()
    {
        if (!isExamining) return;

        isExamining = false;
        hasOpenedCover = false;

        StartCoroutine(ExitExaminationCoroutine());

        if (bookAnimator)
            bookAnimator.SetBool("isOpen", false);
    }

    public void NextPage()
    {
        if (!isExamining) return;

        if (!hasOpenedCover)
        {
            hasOpenedCover = true;
            if (bookAnimator) bookAnimator.SetBool("isOpen", true);
            return;
        }

        if (currentPage < totalPages)
        {
            if (bookAnimator) bookAnimator.SetTrigger("NextPage");
            currentPage++;
        }
    }

    public void PreviousPage()
    {
        if (!isExamining) return;

        if (currentPage > 0)
        {
            if (bookAnimator) bookAnimator.SetTrigger("PrevPage");
            currentPage--;
        }
    }

    public void ToggleExamination()
    {
        if (isExamining)
            Close();
        else
            Open();
    }

    private IEnumerator EnterExaminationCoroutine()
    {
        if (player) player.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        originalPosition = mainCamera.transform.position;
        originalRotation = mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(originalPosition, examinationCamera.transform.position, curveValue);
            mainCamera.transform.rotation = Quaternion.Lerp(originalRotation, examinationCamera.transform.rotation, curveValue);

            yield return null;
        }
        mainCamera.transform.position = examinationCamera.transform.position;
        mainCamera.transform.rotation = examinationCamera.transform.rotation;

        if (mainCamera) mainCamera.enabled = false;
        examinationCamera.SetActive(true);
    }

    private IEnumerator ExitExaminationCoroutine()
    {
        examinationCamera.SetActive(false);
        if (mainCamera) mainCamera.enabled = true;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(examinationCamera.transform.position, originalPosition, curveValue);
            mainCamera.transform.rotation = Quaternion.Lerp(examinationCamera.transform.rotation, originalRotation, curveValue);
            yield return null;
        }
        mainCamera.transform.position = originalPosition;
        mainCamera.transform.rotation = originalRotation;

        if (player) player.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
