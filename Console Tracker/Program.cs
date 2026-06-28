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

            foreach (var app in config.Applications)
            {
                Console.WriteLine($"{app.DisplayName} | {app.ProcessName} | {app.IsEnabled}");
            }
        }
    }
}
