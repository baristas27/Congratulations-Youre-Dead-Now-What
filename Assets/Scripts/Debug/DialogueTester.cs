using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private SoulDialogueController dialogueController;

    //private void Start()
    //{
    //    List<string> testSegments = new List<string>
    //    {
    //        "Ben bir deðirmenciydim.",
    //        "Bir gün çatýdan düþerek öldüm.",
    //        "Ama bunun cehennemi hak edecek bir tarafý yok."
    //    };

    //    dialogueController.StartDialogue(testSegments);
    //}

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && dialogueController.IsDialogueActive)
        {
            dialogueController.ShowNextSegment();
        }
    }
}