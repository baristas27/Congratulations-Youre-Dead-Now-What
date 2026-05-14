using TMPro;
using UnityEngine;

public class PhysicalCaseFileView : MonoBehaviour
{
    [Header("Page Roots")]
    [SerializeField] private GameObject page1Root;
    [SerializeField] private GameObject page2Root;

    [Header("Page 1 Text Fields")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI causeOfDeathText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Page 2 Text Fields")]
    [SerializeField] private TextMeshProUGUI summaryText;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI promptText;

    private int currentPageIndex;

    public void SetSoulData(SoulData soul)
    {
        if (soul == null)
        {
            Clear();
            return;
        }

        if(nameText != null)
        {
            nameText.text = $"Name: {soul.soulName}";
        }

        if (ageText != null)
        {
            ageText.text = $"Age: {soul.age}";
        }

        if (causeOfDeathText != null)
        {
            causeOfDeathText.text = $"Cause Of Death: \n{soul.causeOfDeath}";
        }

        if  (statusText != null)
        {
            statusText.text = $"Status: Pending Review";
        }

        if (summaryText != null)
        {
            summaryText.text = $"Official Summary: \n{soul.fileSummary}";
        }

        ShowPage(0);
    }

    public void ShowPage(int pageIndex)
    {
        currentPageIndex = Mathf.Clamp(pageIndex, 0, 1);

        if (page1Root != null)
        {
            page1Root.SetActive(currentPageIndex == 0);
        }

        if (page2Root != null)
        {
            page2Root.SetActive(currentPageIndex == 1);
        }
    }

    public void ShowNextPage()
    {
        ShowPage(currentPageIndex + 1);
    }

    public void ShowPreviousPage()
    {
        ShowPage(currentPageIndex - 1);
    }

    public void SetPromptVisible(bool visible)
    {
        if (promptText == null) return;

        promptText.gameObject.SetActive(visible);

        if (visible )
        {
            promptText.text = "A / D - Page      E / Esc - Close";
        }
    }

    public void Clear()
    {
        if (nameText != null)
            nameText.text = "NAME: —";

        if (ageText != null)
            ageText.text = "AGE: —";

        if (causeOfDeathText != null)
            causeOfDeathText.text = "CAUSE OF DEATH:\n—";

        if (statusText != null)
            statusText.text = "STATUS: NO ACTIVE CASE";

        if (summaryText != null)
            summaryText.text = "OFFICIAL SUMMARY:\n—";

        ShowPage(0);
        SetPromptVisible(false);
    }

}
