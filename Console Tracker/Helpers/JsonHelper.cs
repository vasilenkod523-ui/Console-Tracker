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

        public static void SaveConfig(TrackerConfiguration config)
        {
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));

            // сериализуй config в строку и запиши в файл
            // подсказка: JsonSerializer.Serialize() и File.WriteAllText()
        }

        public static  TrackerConfiguration LoadConfig()
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
            // подсказка: File.Exists(), File.ReadAllText(), JsonSerializer.Deserialize<>()
      

        }
    }
}
