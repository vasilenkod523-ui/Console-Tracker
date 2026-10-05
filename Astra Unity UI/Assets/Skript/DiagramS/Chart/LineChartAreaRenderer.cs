using MethStatisticPie;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class LineChartAreaRenderer : MonoBehaviour
{
    [SerializeField] private ChartDataProvider dataProvider;
    [SerializeField] private LineChart lineChart;

    private void Awake()
    {
        if (lineChart == null) lineChart = GetComponent<LineChart>();
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
        if (lineChart == null) return;

        // 1. Очищаем старые линии и подписи на оси X
        lineChart.ClearData();
        var xAxis = lineChart.GetChartComponent<XAxis>();
        if (xAxis != null) xAxis.ClearData();

        if (data == null || data.Count == 0)
        {
            lineChart.RefreshChart();
            return;
        }

        // 2. Проверяем наличие серии. Если нет — создаем и включаем заливку (Area)
        if (lineChart.series.Count == 0)
        {
            var serie = lineChart.AddSerie<Line>("Serie1");
            // Включаем отображение заливки под линией из кода
            serie.areaStyle.show = true;
        }

        // 3. Заполняем график
        foreach (var item in data)
        {
            // Название приложения на нижнюю ось
            lineChart.AddXAxisData(item.Name);
            // Точка на графике
            lineChart.AddData(0, item.Minutes);
        }

        lineChart.RefreshChart();
    }
}