using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulDialogueController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;


    [Header("UI Presentation")]
    [SerializeField] private SoulDialogueUI dialogueUI;

    private List<string> currentSegments;
    private int currentIndex;
    private Action onDialogueFinished;
    private bool isDialogueActive;

    public bool IsDialogueActive => isDialogueActive;

    public void StartDialogue(List<string> segments, Action finishedCallback = null)
    {
        if (dialoguePanel !=null)
        {
            dialoguePanel.SetActive(true);
        }

        if(segments == null || segments.Count == 0)
        {
            finishedCallback?.Invoke();
            return;
        }
        currentSegments = segments;
        currentIndex = 0;
        onDialogueFinished = finishedCallback;
        isDialogueActive = true;

        if (dialogueUI != null)
        {
            dialogueUI.OnSegmentFinished += HandleSegmentFinished;
            dialogueUI.ShowSegment(currentSegments[currentIndex]);
        }

    }

    private void HandleSegmentFinished()
    {
    //   currentIndex++;
    
    //if (currentIndex >= currentSegments.Count)
    //{
    //    EndDialogue();
    //    return;
    //}
    
    //if (dialogueUI != null)
    //    dialogueUI.ShowSegment(currentSegments[currentIndex]);
    }

    public void ShowNextSegment()
    {
        if (!isDialogueActive || dialogueUI == null || dialogueUI.IsTyping)
            return;

        currentIndex++;

        if (currentIndex >= currentSegments.Count)
        {
            EndDialogue();
            return;
        }

        if (dialogueUI != null)
            dialogueUI.ShowSegment(currentSegments[currentIndex]);
    }

    public void ShowCurrentSegment()
    {
        dialogueText.text = currentSegments[currentIndex];
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        if (dialogueUI != null)
        {
            dialogueUI.OnSegmentFinished -= HandleSegmentFinished;
            dialogueUI.ClearDialogue();
        }

        dialoguePanel.SetActive(false);
        dialogueText.text = string.Empty;

        onDialogueFinished?.Invoke();
    }





}
