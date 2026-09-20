using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;

public class UserInstalledAppsReader : MonoBehaviour
{
    public class AppInfo
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public string DisplayVersion { get; set; }
        public string InstallLocation { get; set; }

    }

    void Start()
    {
        List<AppInfo> apps = GetUserInstalledApplications();
        Debug.Log($"Найдено программ текущего пользователя: {apps.Count}");
        foreach (var app in apps)
        {
            Debug.Log($"[User App] {app.DisplayName} | Процесс: {app.ProcessName ?? "не угадано"} | Версия: {app.DisplayVersion}");
        }

    }
    public static List<AppInfo> GetUserInstalledApplications()
    {
        var apps = new List<AppInfo>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Обращаемся ТОЛЬКО к ветке текущего пользователя
        using (RegistryKey baseKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
        //LocalMachine.OpenSubKey  
        { 
            if (baseKey == null) return apps;

            foreach (string subkeyName in baseKey.GetSubKeyNames())
            {
                using (RegistryKey subkey = baseKey.OpenSubKey(subkeyName))
                {
                    if (subkey == null) continue;

                    // Пропускаем системные компоненты (на всякий случай)
                    if (Convert.ToInt32(subkey.GetValue("SystemComponent", 0)) == 1) continue;
                    if (subkey.GetValue("ParentDisplayName") != null) continue;

                    string displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrWhiteSpace(displayName)) continue;

                    // Проверяем наличие деинсталлятора
                    string uninstallString = subkey.GetValue("UninstallString") as string;
                    if (string.IsNullOrEmpty(uninstallString)) continue;

                    // Убираем дубликаты
                    if (seenNames.Contains(displayName)) continue;
                    seenNames.Add(displayName);
//--------------------------------------------------------------------------------------
                    // ↓ новая строка — читаем путь к иконке
                    string iconPath = subkey.GetValue("DisplayIcon") as string;

                    string installLocation = subkey.GetValue("InstallLocation") as string ?? "";

                    // ↓ новая строка — вызываем угадывание прямо здесь
                    string guessedProcessName = GuessProcessName(installLocation, iconPath);

                    apps.Add(new AppInfo
                    {
                        DisplayName = displayName,
                        DisplayVersion = subkey.GetValue("DisplayVersion") as string ?? "Н/Д",
                        InstallLocation = installLocation,
                        ProcessName = guessedProcessName   // ← новое поле заполняется
                    });
                }
            }
        }

        // Сортировка по алфавиту
        apps.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        return apps;
    }

    public static string GuessProcessName(string installLocation, string iconPath)
    {
        // Способ 1: DisplayIcon в реестре часто указывает прямо на exe
        if (!string.IsNullOrEmpty(iconPath))
        {
            // DisplayIcon может выглядеть как "C:\...\app.exe,0" — отрезаем ",0"
            string cleanPath = iconPath.Split(',')[0].Trim();

            if (cleanPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                && File.Exists(cleanPath))
            {
                return Path.GetFileNameWithoutExtension(cleanPath);
            }
        }
        // Способ 2: ищем любой .exe прямо в папке InstallLocation
        if (!string.IsNullOrEmpty(installLocation) && Directory.Exists(installLocation))
        {
            try
            {
                string[] exeFiles = Directory.GetFiles(installLocation, "*.exe", SearchOption.TopDirectoryOnly);

                if (exeFiles.Length > 0)
                {
                    return Path.GetFileNameWithoutExtension(exeFiles[0]);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Нет доступа к папке — пропускаем
            }
        }
        return null; 
    }

}