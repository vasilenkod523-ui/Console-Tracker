using MethStatisticPie;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class BarChartRenderer : MonoBehaviour
{
    [SerializeField] private ChartDataProvider dataProvider;
    [SerializeField] private BarChart barChart;

    private void Awake()
    {
        if (barChart == null) barChart = GetComponent<BarChart>();
    }

    private void OnEnable()
    {
        if (dataProvider != null) dataProvider.OnDataUpdated += Render;
    }

    private void OnDisable()
    {
        if (dataProvider != null) dataProvider.OnDataUpdated -= Render;
    }

    private void Render(List<ChartDataItem> data)
    {
        if (barChart == null) return;

        // 1. Очищаем и данные, и названия на оси X
        barChart.ClearData();
        var xAxis = barChart.GetChartComponent<XAxis>();
        if (xAxis != null) xAxis.ClearData();

        if (data == null || data.Count == 0)
        {
            barChart.RefreshChart();
            return;
        }

        // 2. Автоматически создаем серию, если в Инспекторе её забыли добавить
        if (barChart.series.Count == 0)
        {
            barChart.AddSerie<Bar>("Serie1");
        }

        // 3. Заполняем осей и столбцы
        foreach (var item in data)
        {
            barChart.AddXAxisData(item.Name);
            barChart.AddData(0, item.Minutes);
        }

        barChart.RefreshChart();
    }
}