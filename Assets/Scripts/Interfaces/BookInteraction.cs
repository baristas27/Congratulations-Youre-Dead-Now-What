using UnityEngine;

public interface IBookInteractable { void ToggleExamination(); }

public class BookInteraction : MonoBehaviour, IBookInteractable
{
    [Header("Camera Settings")]
    public GameObject examinationCamera; // Kitaba yukarýdan bakan sabit kamera

    private bool isExamining = false;
    private PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();  // Cache
        if (examinationCamera) examinationCamera.SetActive(false);
    }

    public void ToggleExamination()
    {
        isExamining = !isExamining;
        if (isExamining) EnterExamination();
        else ExitExamination();
    }

    private void EnterExamination()
    {
        examinationCamera.SetActive(true);
        if (player) player.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Examination MODU GÝRÝÞ");
    }

    private void ExitExamination()
    {
        examinationCamera.SetActive(false);
        if (player) player.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Examination MODU ÇIKIÞ");
    }
}