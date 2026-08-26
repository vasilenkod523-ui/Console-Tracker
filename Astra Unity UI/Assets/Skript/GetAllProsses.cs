using System;
using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;

public class UserInstalledAppsReader : MonoBehaviour
{
    public class AppInfo
    {
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string InstallLocation { get; set; }
    }

    void Start()
    {
        Debug.Log($"ривышаириыгвигащрывагщ");
        List<AppInfo> apps = GetUserInstalledApplications();
        Debug.Log($"Найдено программ текущего пользователя: {apps.Count}");
        foreach (var app in apps)
        {
            Debug.Log($"[User App] {app.DisplayName} | Версия: {app.DisplayVersion}");
        }
    }
    public static List<AppInfo> GetUserInstalledApplications()
    {
        var apps = new List<AppInfo>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Обращаемся ТОЛЬКО к ветке текущего пользователя
        using (RegistryKey baseKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
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

                    apps.Add(new AppInfo
                    {
                        DisplayName = displayName,
                        DisplayVersion = subkey.GetValue("DisplayVersion") as string ?? "Н/Д",
                        InstallLocation = subkey.GetValue("InstallLocation") as string ?? ""
                    });
                }
            }
        }

        // Сортировка по алфавиту
        apps.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        return apps;
    }
}