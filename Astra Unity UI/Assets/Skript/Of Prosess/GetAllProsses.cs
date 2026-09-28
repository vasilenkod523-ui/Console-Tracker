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
    }

    //----------------------------------------------------------------

    public static List<AppInfo> GetUserInstalledApplications()
    {
        var apps = new List<AppInfo>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1) Программы, установленные только для текущего пользователя
        ScanUninstallKey(Registry.CurrentUser, apps, seenNames);

        // 2) Программы для всех пользователей, 64-бит ветка реестра
        using (RegistryKey hklm64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
            ScanUninstallKey(hklm64, apps, seenNames);
        }

        // 3) Программы для всех пользователей, 32-бит ветка (WOW6432Node)
        using (RegistryKey hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
        {
            ScanUninstallKey(hklm32, apps, seenNames);
        }

        // Сортировка по алфавиту
        apps.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        return apps;
    }

    // Ключевые слова, по которым отсеивается технический "мусор": рантаймы,
    // редистрибутивы, драйверы и служебные компоненты — не то, чем пользователь
    // реально "пользуется". Список можно дополнять под себя.
    private static readonly string[] NoiseKeywords =
    {
        "redistributable",
        "runtime",
        "update for",
        "security update",
        "hotfix",
        "driver",
        " sdk",
        "software development kit",
        "maintenance service",
        "language pack",
        "visual c++",
        "directx",
        ".net framework",
        ".net core",
        ".net host",
        ".net sdk",
        "asp.net",
        "webview2",
        "edge update",
        "physx",
        "hd audio",
        "usb driver",
        "microsoft"
    };

    private static bool IsNoise(string displayName)
    {
        string lower = displayName.ToLowerInvariant();
        foreach (var keyword in NoiseKeywords)
        {
            // .ToLowerInvariant() и тут тоже — чтобы регистр слова в списке
            // не имел значения, как бы вы его ни вписали
            if (lower.Contains(keyword.ToLowerInvariant())) return true;
        }
        return false;
    }

    //----------------------------------------------------------------

    private static void ScanUninstallKey(RegistryKey hive, List<AppInfo> apps, HashSet<string> seenNames)
    {



        using (RegistryKey baseKey = hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
        { 
            if (baseKey == null) return;

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

                    if (IsNoise(displayName)) continue;
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
                        ProcessName = guessedProcessName  
                    });
                }
            }
        }
    }

    public static string GuessProcessName(string installLocation, string iconPath)
    {
        // Способ 1: DisplayIcon в реестре часто указывает прямо на exe
        if (!string.IsNullOrEmpty(iconPath))
        {
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