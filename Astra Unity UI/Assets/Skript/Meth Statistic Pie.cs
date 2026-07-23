using System.IO;
using System.Linq;
using UnityEngine;
using XCharts;
using XCharts.Runtime;

[System.Serializable]
public class ApplicationItem
{
    public string DisplayName;
    public string ProcessName;
    public bool IsEnabled;
}

[System.Serializable]
public class ApplicationConfig
{
    public ApplicationItem[] Applications;
}

[System.Serializable]
public class UsageTime
{
    public int Duration;
}

[System.Serializable]
public class ProcessUsage
{
    public string ProcessName;
    public UsageTime[] UsageTimes;
}

[System.Serializable]
public class ProcessUsageList
{
    public ProcessUsage[] Items;
}


public class TimeTrackerPieChart : MonoBehaviour
{
    public PieChart pieChart;

    public string applicationsJsonPath;
    public string usageJsonPath;


    void Start()
    {
        LoadChart();
    }


    void LoadChart()
    {
        string applicationsJson = File.ReadAllText(applicationsJsonPath);
        string usageJson = File.ReadAllText(usageJsonPath);


        // Читаем список приложений
        ApplicationConfig applications =
            JsonUtility.FromJson<ApplicationConfig>(applicationsJson);


        // JsonUtility не умеет читать массив напрямую,
        // поэтому добавляем обёртку
        ProcessUsage[] usages =
            JsonUtility.FromJson<ProcessUsageList>(
                "{\"Items\":" + usageJson + "}"
            ).Items;


        pieChart.ClearData();


        foreach (var app in applications.Applications)
        {
            var process = usages.FirstOrDefault(
                x => x.ProcessName == app.ProcessName
            );


            if (process == null)
                continue;


            int totalSeconds = process.UsageTimes.Sum(
                x => x.Duration
            );


            // В диаграмму отдаём часы
            float hours = totalSeconds / 3600f;


            pieChart.AddData(
                0,
                hours,
                app.DisplayName
            );


            Debug.Log(
                app.DisplayName +
                ": " +
                FormatTime(totalSeconds)
            );
        }
    }


    string FormatTime(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int secs = seconds % 60;

        return $"{hours}ч {minutes}м {secs}с";
    }
}
