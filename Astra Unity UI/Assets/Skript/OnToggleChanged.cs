using System.IO;
using Console_Tracker.Models;
using Newtonsoft.Json;
using UnityEngine;

public class OnToggleChanged : MonoBehaviour    
{
    private static string configPath = "D:\\WEB\\Astra\\Json Saver\\config.json";

    public void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {

            //AddApplication("ksgo", "ksgo", true);
            Debug.Log($"Toggle is ON");
            // Add your logic for when the toggle is turned on
        }
        else
        {

            //AddApplication("ksgo", "ksgo", false);
            Debug.Log($"Toggle is OFF ");
            // Add your logic for when the toggle is turned off
        }
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
