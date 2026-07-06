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
        private static string statisticsPath = "statistics.json";

        public static void SaveConfig(TrackerConfiguration config)
        {
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));

            // сериализуй config в строку и запиши в файл
            // подсказка: JsonSerializer.Serialize() и File.WriteAllText()
        }
        // на Этапе 8 реализуй метод SaveStatistics, который будет сериализовывать объект StatisticApp в строку и записывать в файл statistics.json.
        //public static void SaveStatistics(StatisticApp statistics)
        //{
        //    File.WriteAllText(statisticsPath, JsonSerializer.Serialize(statistics));
        //    // сериализуй statistics в строку и запиши в файл
        //}

        public static TrackerConfiguration LoadConfig()
        {
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<TrackerConfiguration>(json); //?????
            }
            else
            {
                return new TrackerConfiguration();
            }
            // если файл существует - прочитай и десериализуй
            // если нет - верни new TrackerConfiguration()
           
      

        }
    }
}
