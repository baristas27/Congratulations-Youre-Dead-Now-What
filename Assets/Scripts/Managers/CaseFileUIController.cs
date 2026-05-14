using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the case file UI panel shown after a soul's intro dialogue ends.
/// Displays the soul's file summary and exposes Heaven / Hell judgment buttons.
/// </summary>
public class CaseFileUIController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject filePanel;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI titleText;   // Optional — can be left unassigned
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Judgment Buttons")]
    [SerializeField] private Button heavenButton;
    [SerializeField] private Button hellButton;

    private Action<bool> onVerdictGiven;

    private void Awake()
    {
        heavenButton.onClick.AddListener(() => SubmitVerdict(true));
        hellButton.onClick.AddListener(() => SubmitVerdict(false));

        filePanel.SetActive(false);
    }

    /// <summary>
    /// Opens the case file panel, populates it with soul data,
    /// and registers the callback that fires when the player makes a judgment.
    /// verdictCallback receives true for Heaven, false for Hell.
    /// </summary>
    public void Open(SoulData soul, Action<bool> verdictCallback)
    {
        // file open sound
        if (TryGetComponent<DialogueAudioHandler>(out var audioHandler))
        {
            audioHandler.PlayFileOpenSound();
        }

        onVerdictGiven = verdictCallback;

        if (titleText != null)
            titleText.text = $"CASE FILE — {soul.soulName}";

        bodyText.text = soul.fileSummary;

        filePanel.SetActive(true);
    }

    /// <summary>
    /// Closes the case file panel and clears the pending verdict callback.
    /// </summary>
    public void Close()
    {
        filePanel.SetActive(false);
        onVerdictGiven = null;
    }

    private void SubmitVerdict(bool sentToHeaven)
    {
        onVerdictGiven?.Invoke(sentToHeaven);
    }
}
