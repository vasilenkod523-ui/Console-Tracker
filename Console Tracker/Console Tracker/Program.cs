using Console_Tracker.Helpers;
using Console_Tracker.Models;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;



namespace Console_Tracker
{
    internal class Program
    {
        
        // Импорт функций из системной библиотеки user32.dll,
        // т.к. в .NET нет своих методов для получения активного окна ОС.

        // Возвращает хендл (дескриптор) окна, которое сейчас в фокусе у пользователя
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // По хендлу окна возвращает ID процесса (через out-параметр processId),
        // которому это окно принадлежит
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        static void Main(string[] args)
        {
            
            ProgramScanner.ScanAndSave();

            var config = JsonHelper.LoadConfig();

            List<StatisticApp> statistic;
            string statisticFilePath = "D:\\WEB\\Astra\\Json Saver\\statistic.json";

            if (File.Exists(statisticFilePath))
            {
                var statisticJson = File.ReadAllText(statisticFilePath);
                statistic = JsonSerializer.Deserialize<List<StatisticApp>>(statisticJson) ?? new List<StatisticApp>();
            }
            else
            {
                statistic = new List<StatisticApp>();
            }

            if (!config.IsTrackingEnabled)
                return;

            // Оставляем из конфига только те приложения, у которых стоит IsEnabled = true —
            // именно их будем искать среди активных процессов
            var enabledApps = config.Applications
                .Where(a => a.IsEnabled)
                .ToList();

            foreach (var app in enabledApps)
            {
                var appStatistic = statistic?.Find(s => s.ProcessName == app.ProcessName);
                // Считаем общую сумму секунд из прошлых сессий (если они есть)
                int totalSeconds = appStatistic?.UsageTimes.Sum(t => t.Duration) ?? 0;

                Console.WriteLine($"- {app.DisplayName} ({app.ProcessName}) | Наиграно: {totalSeconds / 60} мин.");
            }

            //_______________________________________________________________________________________________________________

            // Храним хендл предыдущего активного окна, чтобы реагировать
            // только на смену фокуса, а не проверять на каждой итерации
            IntPtr lastWindowHandle = IntPtr.Zero;

            Models.TimeSpan? currentTimeSpan = null;
            

            while (true)
            {
                // Через user32 узнаём хендл окна, которое сейчас в фокусе —
                // дальше сравним его с предыдущим, чтобы понять, сменился ли фокус
                IntPtr currentWindowHandle = GetForegroundWindow();

                if (currentWindowHandle != lastWindowHandle)
                {
                    if (currentTimeSpan != null)
                    {
                        currentTimeSpan.EndTime = DateTime.Now;
                        currentTimeSpan.Duration =
                            (int)(currentTimeSpan.EndTime - currentTimeSpan.StartTime).TotalSeconds;

                        var json = JsonSerializer.Serialize(statistic);
                        File.WriteAllText(statisticFilePath, json);

                        currentTimeSpan = null;
                    }

                    // фокус сменился на другое окно
                    uint processId;
                    
                    GetWindowThreadProcessId(currentWindowHandle, out processId);
                    try
                    {
                        // Может выбросить ArgumentException, если процесс уже успел закрыться
                        Process process = Process.GetProcessById((int)processId);

                        // Ищем совпадение: есть ли активный процесс среди включённых в config.json.
                        // Сравнение без учёта регистра, т.к. Windows не всегда даёт имена в одном регистре
                        var matchedApp = enabledApps.FirstOrDefault(a =>
                            a.ProcessName.Equals(process.ProcessName, StringComparison.OrdinalIgnoreCase));

                        if (matchedApp != null)
                        {
                            //это отслеживаемое приложение
                            currentTimeSpan = new Models.TimeSpan
                            {
                                StartTime = DateTime.Now
                            };

                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Активно: {matchedApp.DisplayName}");
                            //------???????-----!!!!!!!--------
                            // find existing statistic entry for this process
                            var appStatistic = statistic?.Find(s =>
                                s.ProcessName.Equals(matchedApp.ProcessName, StringComparison.OrdinalIgnoreCase));

                            if (appStatistic == null)
                            {
                                // create new statistic entry if none exists
                                appStatistic = new StatisticApp
                                {
                                    ProcessName = matchedApp.ProcessName,
                                    UsageTimes = new List<Models.TimeSpan>()
                                };
                                statistic?.Add(appStatistic);
                            }

                            // record new timespan
                            appStatistic.UsageTimes.Add(currentTimeSpan);
                            //------???????-----!!!!!!!--------
                        }

                        else
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {process.ProcessName} (не отслеживается)");
                        }
                    }
                    catch (ArgumentException)
                    {
                        // процесс уже мог завершиться к моменту проверки
                        Console.WriteLine("Не удалось определить процесс");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }

                    lastWindowHandle = currentWindowHandle;
                }
                Thread.Sleep(1000);
            }
        }
    }
}