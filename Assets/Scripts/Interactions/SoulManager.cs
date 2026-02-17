using System.Collections.Generic;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<SoulData> todaySouls = new();

    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Debug")]
    [SerializeField] private bool destroyPreviousOnSpawn = true;

    private int currentSoulIndex;
    private GameObject currentSoulInstance;

    private void Start()
    {
        SpawnNextSoul();
    }

    [ContextMenu("Spawn Next Soul")]
    public void SpawnNextSoul()
    {
        if (todaySouls == null || todaySouls.Count == 0)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: todaySouls is empty.", this);
            return;
        }

        if (soulPrefab == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: soulPrefab is not assigned.", this);
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: spawnPoint is not assigned.", this);
            return;
        }

        if (currentSoulIndex >= todaySouls.Count)
        {
            Debug.Log("No more souls for today.", this);
            return;
        }

        if (destroyPreviousOnSpawn && currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }

        currentSoulInstance = Instantiate(soulPrefab, spawnPoint.position, spawnPoint.rotation);

        var appearance = currentSoulInstance.GetComponent<SoulAppearance>();
        if (appearance == null)
        {
            Debug.LogWarning($"{nameof(SoulManager)}: Spawned prefab has no SoulAppearance component.", currentSoulInstance);
        }
        else
        {
            appearance.ApplySoulVisuals(todaySouls[currentSoulIndex]);
        }

        currentSoulIndex++;
    }

    [ContextMenu("Reset Day Souls")]
    public void ResetDaySouls()
    {
        currentSoulIndex = 0;

        if (currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }

        Debug.Log($"{nameof(SoulManager)}: Reset completed.", this);
    }
}
