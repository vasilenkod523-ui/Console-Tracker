using Console_Tracker.Models;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ToggleChartController : MonoBehaviour 
{
    private static string configPath = "D:\\WEB\\Astra\\Json Saver\\config.json";
    public static void SaveConfig(TrackerConfiguration config)
    {
        var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
        using var sw = new StringWriter();
        serializer.Serialize(sw, config);
        File.WriteAllText(configPath, sw.ToString());
        // ???????????????????????????? 
    }
    public void AddSegment()
    {
        // Код для добавления сегмента в диаграмму 

    }
    public void RemoveSegment()
    {
        // Код для удаления сегмента из диаграммы

    }

}
