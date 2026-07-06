using Console_Tracker.Helpers;
using Console_Tracker.Models;
using System.Diagnostics;
using System.Linq;
using System;


namespace Console_Tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = JsonHelper.LoadConfig();
            if (!config.IsTrackingEnabled)
            {
                return;
            }


            foreach (var app in config.Applications)
            {
                var process = Process.GetProcessesByName(app.ProcessName).FirstOrDefault();
                if (process != null)
                {

                }
            }

            //JsonHelper.SaveStatistics(new StatisticApp
            //{
            //    ProcessName = "ExampleApp",
            //    UsageTimes = new List<Models.TimeSpan>
            //{
            //    new Models.TimeSpan
            //    {
            //        StartTime = DateTime.Now,           // replace with actual start
            //        EndTime = DateTime.Now.AddMinutes(1), // replace with actual end
            //        Duration = 60                        // replace with actual duration (int)
            //    }
            //}
            //});
            // не работает, нужно исправить
        }
    }
}




