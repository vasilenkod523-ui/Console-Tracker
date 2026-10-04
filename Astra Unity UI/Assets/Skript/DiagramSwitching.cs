using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChartTab
{
    public Button button;
    public GameObject chartRoot;
}

public class DiagramSwitching : MonoBehaviour
{
    [SerializeField] private ChartTab[] tabs;

    private void Start()
    {
        foreach (var tab in tabs)
        {
            var target = tab;
            tab.button.onClick.AddListener(() => ShowOnly(target));
        }

        ShowOnly(tabs[0]);
    }

    private void ShowOnly(ChartTab selected)
    {
        foreach (var tab in tabs)
        {
            tab.chartRoot.SetActive(tab == selected);
        }
    }
}