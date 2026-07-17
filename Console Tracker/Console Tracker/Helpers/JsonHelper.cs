using Console_Tracker.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Helpers
{
    public static class JsonHelper
    {
        // GetConfig, GetStatistics, SaveStatistics.

        private static string configPath = "config.json";
        private static string statisticPath = "statistic.json";

        public static void SaveConfig(TrackerConfiguration config)
        {
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));
        }
        public static void SaveStatistics(StatisticApp statistics)
        {
            File.WriteAllText(statisticPath, JsonSerializer.Serialize(statistics));
        }

        public static TrackerConfiguration LoadConfig()
        {
            if (File.Exists(configPath))
            {
                var configjson = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<TrackerConfiguration>(configjson) ?? new TrackerConfiguration();
            }
            else
            {
                return new TrackerConfiguration();
            }
            // если файл существует - прочитай и десериализуй
            // если нет - верни new TrackerConfiguration()
        }

        public static List<StatisticApp> LoadStatistics()
        {
            if (File.Exists(statisticPath))
            {
                var statisticJson = File.ReadAllText(statisticPath);
                return JsonSerializer.Deserialize<List<StatisticApp>>(statisticJson) ?? new List<StatisticApp>();
            }
            else
            {
                return new List<StatisticApp>();
            }
        }

    }
}

