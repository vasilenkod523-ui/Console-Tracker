using Console_Tracker.Helpers;
using Console_Tracker.Models;

namespace Console_Tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new TrackerConfiguration();

            config.Applications.Add(new TrackedApplication
            {
                DisplayName = "Google Chrome",
                ProcessName = "chrome",
                IsEnabled = true
            });

            JsonHelper.SaveConfig(config);

            var loaded = JsonHelper.LoadConfig();
            foreach (var app in loaded.Applications)
            {
                Console.WriteLine($"{app.DisplayName} | {app.ProcessName} | {app.IsEnabled}");
            }

            
            ProcessHelper.GetProcessByName("");
        }
    }
}


