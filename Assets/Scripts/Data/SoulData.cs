using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public struct EvidenceData
{
    [Tooltip("The 3D Prefab model to be instantiated on the desk.")]
    public GameObject evidencePrefab;

    [Tooltip("The title displayed in the Inspection UI.")]
    public string evidenceTitle;

    [Tooltip("The standard description visible at first glance.")]
    [TextArea(3, 10)]
    public string initialDescription;

    [Header("Hidden Detail System")]
    [Tooltip("Does this object have a hidden detail to discover?")]
    public bool hasHiddenDetail;

    [Tooltip("The text revealed when the player finds the hidden detail.")]
    [TextArea(2, 5)]
    public string revealedDetailText;

    [Header("Inspection Feedback")]
    [Tooltip("The monologue the player's character says if the object is fully inspected and has no hidden details.")]
    [TextArea(1, 3)]
    public string clerkCleanMonologue; // Örn: "Looks clean to me. Nothing to see here."

    [Header("Reactions")]
    [Tooltip("Vague lines spoken by the soul when the detail is found (e.g., 'What are you doing?').")]
    public List<string> vagueReactions;
}

/// <summary>
/// Represents a spesific point of interrogation within the soul's file.
/// </summary>
[System.Serializable]
public struct InspectionPoint
{
    [Tooltip("The keyword in the text that will be clickable/highlighted.")]
    public string keyword;

    [Tooltip("The response the soul gives when this keyword is interrogated.")]
    [TextArea(2, 5)]
    public string soulAnswer;

    [Tooltip("Does this specific answer reveal a contradiction or sin?")]
    public bool revealsDarkSecret;
}

[CreateAssetMenu(fileName = "NewSoul", menuName = "Purgatory/Soul Data")]
public class SoulData : ScriptableObject
{
    [Header("Identity")]
    public string soulName;
    public int age;
    [TextArea] public string causeOfDeath;

    [Header("Dialogue")]
    [TextArea(2, 5)] public List<string> introDialogueSegments;

    [Header("File Text")]
    [TextArea(5, 15)] public string fileSummary;

    [Header("Appearance Indices (-1 = None)")]
    public int headIndex = -1;
    public int chestIndex = -1;
    public int armsIndex = -1;
    public int beltIndex = -1;
    public int legsIndex = -1;
    public int feetIndex = -1;

    public int noseIndex = -1;
    public int hairIndex = -1;
    public int faceHairIndex = -1;
    public int eyesIndex = -1;
    public int eyebrowsIndex = -1;
    public int earsIndex = -1;

    [Header("Verdict")]
    public bool belongsInHeaven;

    [Header("Interrogation Data")]
    [Tooltip("List of clickable keywords and their associated responses.")]
    public List<InspectionPoint> inspectionPoints; 

    [Header("Evidence Data")]
    [Tooltip("The physical item and inspection details associated with this soul.")]
    public EvidenceData evidence; 
}
