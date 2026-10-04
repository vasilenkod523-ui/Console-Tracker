using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using MethStatisticPie;
using System;

public class PieChartRenderer : MonoBehaviour
{
    [SerializeField] private ChartDataProvider dataProvider;
    [SerializeField] private PieChart pieChart;

    private void OnEnable()
    {
        dataProvider.OnDataUpdated += Render;
    }

    private void OnDisable()
    {
        dataProvider.OnDataUpdated -= Render;
    }

    private void Render(List<ChartDataItem> data)
    {
        if (pieChart?.series == null || pieChart.series.Count == 0)
        {
            Debug.LogError("У PieChart нет ни одной Serie!");
            return;
        }

        var serie = pieChart.series[0];
        serie.ClearData();

        foreach (var item in data)
            serie.AddYData(item.Minutes, item.Name);

        pieChart.RefreshChart();
    }
}
