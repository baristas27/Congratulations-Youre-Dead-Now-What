using UnityEngine;

[CreateAssetMenu(fileName = "NewSoul", menuName = "Purgatory/Soul Data")]
public class SoulData : ScriptableObject
{
    [Header("Identity")]
    public string soulName;
    public int age;
    [TextArea] public string causeOfDeath;

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
}
