using System.Collections;
using UnityEngine;

public interface IBookInteractable { void ToggleExamination(); }

public class BookInteraction : MonoBehaviour, IReadable
{

    [SerializeField] private Animator bookAnimator;




    [SerializeField] private Transform examinationCameraTarget;
    [SerializeField] private PlayerPresentationStateMachine presentationStateMachine;



    private bool isExamining = false;


    private int totalPages = 2;
    private int currentPage = 0;
    private bool hasOpenedCover;

    private void Start()
    {
        if (presentationStateMachine == null)
            presentationStateMachine = FindAnyObjectByType<PlayerPresentationStateMachine>();
    }

    public void Open()
    {
        if (isExamining) return;

        isExamining = true;
        hasOpenedCover = false;

        if (presentationStateMachine != null)
            presentationStateMachine.EnterBookExamination(examinationCameraTarget);
    }

    public void Close()
    {
        if (!isExamining) return;

        isExamining = false;
        hasOpenedCover = false;

        if (bookAnimator)
            bookAnimator.SetBool("isOpen", false);

        if (presentationStateMachine != null)
            presentationStateMachine.ExitBookExamination();
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

  

 
}
