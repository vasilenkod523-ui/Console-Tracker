using Console_Tracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class TimeTrackerPieChart : MonoBehaviour
{
    [SerializeField] private PieChart pieChart; 

    private static string statisticPath =
        "D:\\WEB\\Astra\\Console Tracker\\Console Tracker\\bin\\Debug\\net10.0\\statistic.json";

    private void Start()
    {
        LoadAndDisplayChart();
    }

    private void LoadAndDisplayChart()
    {
        if (!File.Exists(statisticPath))
        {
            Debug.Log("Файл статистики не найден");
            return;
        }

        string statisticJson = File.ReadAllText(statisticPath);
        List<StatisticApp> items = JsonConvert.DeserializeObject<List<StatisticApp>>(statisticJson);

        if (items == null || items.Count == 0)
        {
            Debug.Log("Данные пустые");
            return;
        }

        if (pieChart?.series == null || pieChart.series.Count == 0)
        {
            Debug.LogError("У PieChart нет ни одной Serie!");
            return;
        }

        var serie = pieChart.series[0];
        serie.ClearData(); //????

        foreach (var app in items)
        {
            double totalDuration = app.UsageTimes.Sum(u => u.Duration);
            double totalDurationInMinutes = Math.Ceiling(totalDuration / 60); // Преобразуем в минуты
            serie.AddYData(totalDurationInMinutes, app.ProcessName);
        }
        //

        pieChart.RefreshChart();
      
    }
}