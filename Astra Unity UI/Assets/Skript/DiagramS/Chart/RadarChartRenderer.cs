using MethStatisticPie;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class RadarChartRenderer : MonoBehaviour
{
    [SerializeField] private ChartDataProvider dataProvider;
    [SerializeField] private RadarChart radarChart;

    private void Awake()
    {
        if (radarChart == null) radarChart = GetComponent<RadarChart>();
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
        if (radarChart == null) return;
        if (data == null || data.Count == 0) return;

        // 1. Безопасно получаем компонент оси радара
        var radarCoord = radarChart.GetChartComponent<RadarCoord>();
        if (radarCoord == null) return;

        // 2. Очищаем старые индикаторы
        radarCoord.indicatorList.Clear();

        double max = data.Max(d => d.Minutes);
        if (max <= 0) max = 1; // Защита от нулевого максимума

        // 3. Заполняем очертания (уголки) радара
        foreach (var item in data)
        {
            radarCoord.indicatorList.Add(new RadarCoord.Indicator
            {
                name = item.Name,
                max = max
            });
        }

        // 4. Проверяем наличие серии, создаем при отсутствии
        if (radarChart.series.Count == 0)
        {
            radarChart.AddSerie<Radar>("Serie1");
        }

        var serie = radarChart.series[0];
        serie.ClearData();

        // 5. Загоняем массив значений в форму
        List<double> values = data.Select(d => d.Minutes).ToList();
        serie.AddData(values);

        radarChart.RefreshChart();
    }
}