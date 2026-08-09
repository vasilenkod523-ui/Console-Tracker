using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class GetAllProsses
{
    public static Dictionary<string, string> GetProcesses()
    {
        var apps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                if (process.MainWindowHandle != IntPtr.Zero && !string.IsNullOrWhiteSpace(process.MainWindowTitle)) //??
                {
                    string processName = process.ProcessName;
                    string displayName = process.MainWindowTitle;

                    apps[processName] = displayName; 
                }
            }
            catch
            {
            }
        }
        return apps;
    }
}
