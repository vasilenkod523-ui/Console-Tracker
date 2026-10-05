using Console_Tracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using XCharts.Runtime;
using System.Text;

namespace MethStatisticPie { 
    public class ChartDataProvider : MonoBehaviour
    {
        private static string statisticPath = "D:\\WEB\\Astra\\Json Saver\\statistic.json";
        private static string configPath = "D:\\WEB\\Astra\\Json Saver\\config.json";

        // сюда подписываются все рендереры диаграмм
        public event Action<List<ChartDataItem>> OnDataUpdated;

        private void Start()
        {
            InvokeRepeating(nameof(LoadAndBroadcast), 0f, 5f);
        }

        private void LoadAndBroadcast()
        {
            if (!File.Exists(statisticPath) || !File.Exists(configPath))
            {
                Debug.Log("Файл статистики или конфигурации не найден");
                return;
            }

            List<StatisticApp> items;
            TrackerConfiguration config;
            try
            {
                items = JsonConvert.DeserializeObject<List<StatisticApp>>(ReadShared(statisticPath));
                config = JsonConvert.DeserializeObject<TrackerConfiguration>(ReadShared(configPath));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Не удалось прочитать файлы, попробую в следующий раз: {e.Message}");
                return;
            }

            if (items == null || items.Count == 0)
            {
                Debug.Log("Данные пустые");
                return;
            }

            var enabled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (config?.Applications != null)
            {
                foreach (var a in config.Applications)
                {
                    if (!a.IsEnabled) continue;
                    if (!string.IsNullOrEmpty(a.ProcessName)) enabled.Add(a.ProcessName);
                    if (!string.IsNullOrEmpty(a.DisplayName)) enabled.Add(a.DisplayName);
                }
            }

            var result = new List<ChartDataItem>();
            foreach (var app in items)
            {
                if (string.IsNullOrEmpty(app.ProcessName) || !enabled.Contains(app.ProcessName))
                    continue;

                double totalDuration = app.UsageTimes.Sum(u => u.Duration);
                double minutes = Math.Ceiling(totalDuration / 60);
                result.Add(new ChartDataItem(app.ProcessName, minutes));
            }

            OnDataUpdated?.Invoke(result);
            Debug.Log("Данные для диаграмм обновлены");
        }

        private static string ReadShared(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }
    }
}