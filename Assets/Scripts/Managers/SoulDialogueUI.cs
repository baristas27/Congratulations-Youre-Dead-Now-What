using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class SoulDialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typewriter Settings")]
    [SerializeField] private float characterDelay = 0.0005f;
    [SerializeField] private float punctuationDelayMultiplier = 2.0f; // Longer pause after punctuation

    [Header("Styling - VT323")]
    [SerializeField] private TMP_FontAsset vt323Font;

    [Header("Audio")]
    [SerializeField] private DialogueAudioHandler audioHandler;

    private Coroutine typewriterCoroutine;
    private string currentFullText = "";

    public event Action OnSegmentFinished;

    public bool IsTyping {  get; private set; }


    /// <summary>
    /// It will be called from the Controller, will display the new segment using a typewriter.
    /// </summary>
     
    public void ShowSegment(string text)
    {
        if (vt323Font != null)

            dialogueText.font = vt323Font;
        if (string.IsNullOrEmpty(text))
        {
            OnSegmentFinished?.Invoke();
            return;
        }

        currentFullText = text;

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        typewriterCoroutine = StartCoroutine(TypewriterCoroutine(text));
    }

    private IEnumerator TypewriterCoroutine(string fullText)
    {
        IsTyping = true;
        dialogueText.text = "";

        for (int i = 0; i<fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            if (audioHandler != null)
                audioHandler.PlayTypewriterClick();
            else
                Debug.LogWarning("AudioHandler is null in SoulDialogueUI!");

            if (IsPunctuation(fullText[i]))
                yield return new WaitForSeconds(characterDelay * punctuationDelayMultiplier);
            else
                yield return new WaitForSeconds(characterDelay);
        }
        IsTyping = false;
        OnSegmentFinished?.Invoke();
    }

    private bool IsPunctuation(char c)
    {
        return c == '.' || c == '!' || c == '?' || c == ',' || c == ';';
    }

    public void ClearDialogue()
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        dialogueText.text = "";
        currentFullText = "";
        IsTyping = false;
    }

}
