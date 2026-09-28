using System.IO;
using Console_Tracker.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class OnToggleChanged : MonoBehaviour    
{
    [SerializeField] private Toggle myToggle;
    private static string configPath = "D:\\WEB\\Astra\\Json Saver\\config.json";
    private ConsoleProcessController processController = new ConsoleProcessController();

    public void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            var appToggleData = myToggle.GetComponent<AppToggleData>();
            string processName = appToggleData != null ? appToggleData.ProcessName : myToggle.name;
            AddApplication(myToggle.name, processName, true);
            Debug.Log($"Toggle is ON");
            // Add your logic for when the toggle is turned on
        }
        else
        {
            var appToggleData = myToggle.GetComponent<AppToggleData>();
            string processName = appToggleData != null ? appToggleData.ProcessName : myToggle.name;
            RemoveApplication(processName);
            Debug.Log($"Toggle is OFF");
        }
        processController.RestartConsoleApp();
    }

    public static void SaveConfig(TrackerConfiguration config)
    {
        File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
    }

    public static TrackerConfiguration LoadConfig()
    {
        if (!File.Exists(configPath))
            return new TrackerConfiguration();

        string json = File.ReadAllText(configPath);
        return JsonConvert.DeserializeObject<TrackerConfiguration>(json);
    }
    public static void RemoveApplication(string processName)
    {
        var config = LoadConfig();
        config.Applications.RemoveAll(app =>
            string.Equals(app.ProcessName, processName, System.StringComparison.OrdinalIgnoreCase));
        SaveConfig(config);
    }

    public static void AddApplication(string displayName, string processName, bool isEnabled)
    {
        var config = LoadConfig();
        config.Applications.Add(new TrackedApplication
        {
            DisplayName = displayName,
            ProcessName = processName,
            IsEnabled = isEnabled
        });
        SaveConfig(config);
    }
}