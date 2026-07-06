using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Console_Tracker.Helpers
{
    public static class ProcessHelper
    {
        public static void GetProcessByName(string search)
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                // если имя процесса начинается с того что ввёл пользователь
                if (process.ProcessName.StartsWith(search, StringComparison.OrdinalIgnoreCase)) //?????
                {
                    Console.WriteLine(process.ProcessName); 
                }
                else
                {
                    try
                    {
                        // если имя процесса не начинается с того что ввёл пользователь, то ищем в MainWindowTitle
                        if (process.MainWindowTitle.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine(process.ProcessName);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Игнорируем процессы, к которым нет доступа
                    }
                }
            }
        }

       

        // GetProcessByName, GetProcessById.
        // спроси у ИИ
    }
}
