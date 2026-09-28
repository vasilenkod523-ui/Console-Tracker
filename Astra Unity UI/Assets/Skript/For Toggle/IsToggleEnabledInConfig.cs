using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TrackerConfiguration
{
    public List<TrackedApplication> Applications = new List<TrackedApplication>();
    public bool IsTrackingEnabled;
}

[System.Serializable]
public class TrackedApplication
{
    public string DisplayName;
    public string ProcessName;
    public bool IsEnabled;
}

public class IsToggleEnabledInConfig : MonoBehaviour
{
    private void Start()
    {
        ShouldDrawToggle();
    }
    [SerializeField] private Toggle myToggle;

    public static string pathConfig = "D:\\WEB\\Astra\\Json Saver\\config.json";

    public void ShouldDrawToggle()
    {
        if (!File.Exists(pathConfig))
            return;
        if (myToggle == null)
            return;

        string json = File.ReadAllText(pathConfig);
        TrackerConfiguration config = JsonUtility.FromJson<TrackerConfiguration>(json);

        if (config == null || config.Applications == null)
            return;
        if (!config.IsTrackingEnabled)
            return;
        foreach (var entry in config.Applications)
        {
            Debug.Log($"toggle.name='{myToggle.name}' | DisplayName='{entry.DisplayName}' | ProcessName='{entry.ProcessName}'");
            if (entry.DisplayName == myToggle.name || entry.ProcessName == myToggle.name)
                myToggle.isOn = entry.IsEnabled;
        }
    }
}
