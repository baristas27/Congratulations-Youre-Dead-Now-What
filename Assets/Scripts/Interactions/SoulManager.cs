using System.Collections.Generic;
using System.Text;
using TMPro;

using UnityEngine;

public class SoulManager : MonoBehaviour
{
    [Header("Daily shift data")]
    public List<SoulData> dailySouls;

    private Queue<SoulData> soulQueue;

    [SerializeField] private PlayerPresentationStateMachine presentationStateMachine;

    public SoulData CurrentSoul { get; private set; }

    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform windowPoint;

    [Header("Day Summary")]
    [SerializeField] private GameObject daySummaryPanel;
    [SerializeField] private TextMeshProUGUI summaryText;

    [Header("Dialogue")]
    [SerializeField] private SoulDialogueController dialogueController;

    [Header("Case File")]
    [SerializeField] private CaseFileUIController caseFileUI;
    [SerializeField] private PhysicalCaseFileView physicalCaseFileView;
    [SerializeField] private FileFolioInteraction fileFolioInteraction;

    [Header("Debug")]
    [SerializeField] private bool destroyPreviousOnSpawn = true;

    private GameObject currentSoulInstance;
    private SoulMover currentSoulMover;

    private int correctVerdictCount;
    private int incorrectVerdictCount;
    private readonly List<string> verdictResults = new();

    private void Start()
    {
        InitializeQueue();
        SpawnNextSoul();

        if (fileFolioInteraction != null)
        {
            fileFolioInteraction.OnReviewFinished += OpenVerdictPanel;
        }
    }

    private void OpenVerdictPanel()
    {
        if (caseFileUI == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: CaseFileUI is not assigned. Cannot open verdict panel.", this);
            return;
        }

        if (CurrentSoul == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: CurrentSoul is null. Cannot open verdict panel.", this);
            return;
        }

        presentationStateMachine.EnterUIInteraction();
        caseFileUI.Open(CurrentSoul, HandleVerdict);
    }

    private void OnDestroy()
    {
        if (fileFolioInteraction != null)
        {
            fileFolioInteraction.OnReviewFinished -= OpenVerdictPanel;
        }
    }

    private void InitializeQueue()
    {
        soulQueue = new Queue<SoulData>();

        if (dailySouls != null)
        {
            foreach (SoulData soul in dailySouls)
            {
                soulQueue.Enqueue(soul);
            }
        }

        Debug.Log($"{nameof(SoulManager)}: Queue initialized. Souls waiting: " + soulQueue.Count);
    }

    [ContextMenu("Spawn Next Soul")]
    public void SpawnNextSoul()
    {
        CleanupCurrentSoul();
        if (soulPrefab == null || spawnPoint == null || windowPoint == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: Prefab, SpawnPoint or WindowPoint is missing!", this);
            return;
        }

        if (soulQueue == null || soulQueue.Count == 0)
        {
            ShowDaySummary();
            return;
        }

        if (destroyPreviousOnSpawn && currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }

        CurrentSoul = soulQueue.Dequeue();

        if (fileFolioInteraction != null)
        {
            fileFolioInteraction.SetAvailableForReview(false);
        }

        if (physicalCaseFileView != null)
        {
            physicalCaseFileView.Clear();
        }

        currentSoulInstance = Instantiate(soulPrefab, spawnPoint.position, spawnPoint.rotation);

        var appearance = currentSoulInstance.GetComponent<SoulAppearance>();
        if (appearance != null)
        {
            appearance.ApplySoulVisuals(CurrentSoul);
        }
        else
        {
            Debug.LogWarning($"{nameof(SoulManager)}: Spawned prefab has no SoulAppearance component.", currentSoulInstance);
        }

        var mover = currentSoulInstance.GetComponent<SoulMover>();
        if (mover != null)
        {
            currentSoulMover = mover;
            currentSoulMover.OnDestinationReached += HandleSoulArrived;

            
            mover.MoveTo(windowPoint);
        }
        else
        {
            Debug.LogWarning($"{nameof(SoulManager)}: Spawned prefab has no SoulMover component.", currentSoulInstance);
        }
    }


    private void ShowDaySummary()
    {
        if (daySummaryPanel != null)
        {
            daySummaryPanel.SetActive(true);
        }

        if (summaryText == null)
        {
            return;
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("SHIFT COMPLETE");
        builder.AppendLine();
        builder.AppendLine($"Correct Judgments: {correctVerdictCount}");
        builder.AppendLine($"Incorrect Judgments: {incorrectVerdictCount}");
        builder.AppendLine();
        builder.AppendLine("Results:");
        builder.AppendLine();

        foreach (string result in verdictResults)
        {
            builder.AppendLine($"- {result}");
        }

        summaryText.text = builder.ToString();
    }
    private void HandleSoulArrived()
    {
        // Safety: Eğer mover hala bağlıysa event'i hemen kaldır
        if (currentSoulMover != null)
        {
            currentSoulMover.OnDestinationReached -= HandleSoulArrived;
        }

        if (CurrentSoul == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: HandleSoulArrived called but CurrentSoul is null.", this);
            return;
        }

        if (dialogueController == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: DialogueController is not assigned.", this);
            return;
        }

        // Ruh pencereye geldi, artık diyalog başlatabiliriz
        dialogueController.StartDialogue(
            CurrentSoul.introDialogueSegments,
            OnDialogueFinished
        );
    }

    private void OnDialogueFinished()
    {
        if (CurrentSoul == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: Dialogue finished but CurrentSoul is null.", this);
            return;
        }

        if (physicalCaseFileView == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: PhysicalCaseFileView is not assigned.", this);
            return;
        }

        if (fileFolioInteraction == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: FileFolioInteraction is not assigned.", this);
            return;
        }

        physicalCaseFileView.SetSoulData(CurrentSoul);
        fileFolioInteraction.SetAvailableForReview(true);

        Debug.Log($"{nameof(SoulManager)}: Case file is now available for review: {CurrentSoul.soulName}", this);
    }

    /// <summary>
    /// Called by CaseFileUIController when the player presses Heaven or Hell.
    /// sentToHeaven is true for Heaven, false for Hell.
    /// </summary>
    private void HandleVerdict(bool sentToHeaven)
    {
        if (CurrentSoul == null) return;

        bool isCorrect = sentToHeaven == CurrentSoul.belongsInHeaven;

        if (isCorrect)
        {
            correctVerdictCount++;
        }
        else
        {
            incorrectVerdictCount++;
        }

        string verdictLabel = sentToHeaven ? "Heaven" : "Hell";
        string resultLabel = isCorrect ? "Correct" : "Incorrect";

        verdictResults.Add(
            $"{CurrentSoul.soulName} → {verdictLabel} ({resultLabel})"
        );


        caseFileUI.Close();
        presentationStateMachine.EnterDeskFocus();
        SpawnNextSoul();
    }

    [ContextMenu("Reset Day Souls")]
    public void ResetDaySouls()
    {
        InitializeQueue();

        if (currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }

        Debug.Log($"{nameof(SoulManager)}: Reset completed.", this);
    }

    private void CleanupCurrentSoul()
    {
        if(currentSoulMover != null)
        {
            currentSoulMover.OnDestinationReached -= HandleSoulArrived;
            currentSoulMover = null;
        }

        if(destroyPreviousOnSpawn && currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }
    }
}