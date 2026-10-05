using UnityEngine;
using System.Diagnostics;

public class ConsoleProcessController
{
    // имя процесса БЕЗ ".exe", как в диспетчере задач
    private const string ProcessName = "Console Tracker";
    // путь нужен только чтобы запустить заново
    private const string ExePath = @"D:\WEB\Astra\Console Tracker\Console Tracker\bin\Release\net10.0\Console Tracker.exe";

    public void RestartConsoleApp()
    {
        KillAll();
        Start();
    }

    private void KillAll()
    {
        foreach (var p in Process.GetProcessesByName(ProcessName))
        {
            try
            {
                p.Kill();
                p.WaitForExit(2000); // ждём до 2 секунд, чтобы старая копия точно закрылась
                UnityEngine.Debug.LogWarning($"удалось open {ProcessName}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"Не удалось закрыть {ProcessName}: {e.Message}");
            }
            finally
            {

                p.Dispose();
            }
        }
    }

    private void Start()
    {
        var psi = new ProcessStartInfo
        {
            FileName = ExePath,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process.Start(psi);
    }
}
