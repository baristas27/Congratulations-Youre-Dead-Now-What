using System.Collections.Generic;
using UnityEngine;

public class SoulAppearance : MonoBehaviour
{
    [Header("Armor Parts")]
    public List<GameObject> heads = new();
    public List<GameObject> chests = new();
    public List<GameObject> arms = new();
    public List<GameObject> belts = new();
    public List<GameObject> legs = new();
    public List<GameObject> feet = new();

    [Header("Face Details Parts")]
    public List<GameObject> noses = new();
    public List<GameObject> hairs = new();
    public List<GameObject> faceHairs = new();
    public List<GameObject> eyes = new();
    public List<GameObject> eyebrows = new();
    public List<GameObject> ears = new();

    public void ApplySoulVisuals(SoulData data)
    {
        if (data == null) return;

        DeactivateAll(heads);
        DeactivateAll(chests);
        DeactivateAll(arms);
        DeactivateAll(belts);
        DeactivateAll(legs);
        DeactivateAll(feet);

        DeactivateAll(noses);
        DeactivateAll(hairs);
        DeactivateAll(faceHairs);
        DeactivateAll(eyes);
        DeactivateAll(eyebrows);
        DeactivateAll(ears);

        ActivateSafe(heads, data.headIndex);
        ActivateSafe(chests, data.chestIndex);
        ActivateSafe(arms, data.armsIndex);
        ActivateSafe(belts, data.beltIndex);
        ActivateSafe(legs, data.legsIndex);
        ActivateSafe(feet, data.feetIndex);

        ActivateSafe(noses, data.noseIndex);
        ActivateSafe(hairs, data.hairIndex);
        ActivateSafe(faceHairs, data.faceHairIndex);
        ActivateSafe(eyes, data.eyesIndex);
        ActivateSafe(eyebrows, data.eyebrowsIndex);
        ActivateSafe(ears, data.earsIndex);
    }

    private void DeactivateAll(List<GameObject> parts)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i] != null)
                parts[i].SetActive(false);
        }
    }

    private void ActivateSafe(List<GameObject> parts, int index)
    {
        if (index < 0) return;
        if (index >= parts.Count) return;

        var target = parts[index];
        if (target != null)
            target.SetActive(true);
    }
}
