using System.IO;
using System.Linq;
using UnityEngine;
using XCharts;
using XCharts.Runtime;

/// <summary>
/// Загружает данные из JSON и отображает их
/// в круговой диаграмме XCharts.
/// </summary>
public class TimeTrackerPieChart : MonoBehaviour
{
    // Перетащи сюда PieChart из Hierarchy.
    [SerializeField] private PieChart chart;

    private string statisticPath = @"D:\WEB\Astra\Console Tracker\Console Tracker\bin\Debug\net10.0\statistic.json";
    private string configPath = @"D:\WEB\Astra\Console Tracker\Console Tracker\bin\Debug\net10.0\config.json";

    private void Start()
    {
        LoadChart();
    }

    /// <summary>
    /// Полностью обновляет диаграмму.
    /// </summary>
    private void LoadChart()
    {
        // Проверяем существование файлов.
        if (!File.Exists(statisticPath))
        {
            Debug.LogError("Не найден Statistic.json");
            return;
        }

        if (!File.Exists(configPath))
        {
            Debug.LogError("Не найден Config.json");
            return;
        }

        // Читаем JSON.
        string applicationsJson = File.ReadAllText(statisticPath);
        string configJson = File.ReadAllText(configPath);

        // ====================================================
        // TODO
        // Здесь нужно вызвать парсер твоего JSON.
        // Пока этот код неизвестен,
        // потому что его уже реализовал ты.
        //
        // Например:
        //
        // var applications = ...
        // var usages = ...
        //
        // ====================================================

        chart.ClearData();

        // ====================================================
        // TODO
        // Здесь должен быть цикл.
        //
        // foreach(...)
        // {
        //     найти приложение;
        //     найти время;
        //     посчитать секунды;
        //     перевести в часы;
        //
        //     chart.AddData(...);
        // }
        // ====================================================
    }

    /// <summary>
    /// Перевод секунд в строку.
    /// </summary>
    private string FormatTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return $"{hours} ч {minutes} мин {seconds} сек";
    }

    /// <summary>
    /// Перевод секунд в часы.
    /// Именно это число используется диаграммой.
    /// </summary>
    private float SecondsToHours(int totalSeconds)
    {
        return totalSeconds / 3600f;
    }
}