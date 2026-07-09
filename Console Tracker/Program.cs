using Console_Tracker.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace Console_Tracker
{
    internal class Program
    {
        // Не трогай оно работает с Божьей силой 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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
                    Console.WriteLine($"\n{app.ProcessName}");
                }
            }
            //___________________________________________________

            IntPtr lastWindowHandle = IntPtr.Zero; // объявляем ДО цикла

            while (true)
            {
                IntPtr currentWindowHandle = GetForegroundWindow();

                if (currentWindowHandle != lastWindowHandle)
                {
                    // фокус сменился на другое окно
                    uint processId;
                    GetWindowThreadProcessId(currentWindowHandle, out processId);

                    try
                    {
                        Process process = Process.GetProcessById((int)processId);
                        Console.WriteLine(process.ProcessName);
                    }
                    catch (ArgumentException)
                    {
                        // процесс уже мог завершиться к моменту проверки
                        Console.WriteLine("Не удалось определить процесс");
                    }

                    lastWindowHandle = currentWindowHandle;
                }

                Thread.Sleep(500); // проверяем раз в 0.5 секунды
            }

            //___________________________________________________
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




