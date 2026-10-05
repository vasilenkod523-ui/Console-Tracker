﻿// ============================================================
// ProgramScanner.cs
// Часть КОНСОЛЬНОГО приложения (не Unity!).
//
// Требуется NuGet-пакет, если проект на .NET Core/.NET 5+:
//   dotnet add package Microsoft.Win32.Registry
// (в .NET Framework он не нужен, там реестр доступен «из коробки»)
//
// Задача: прочитать реестр Windows, отфильтровать системные
// компоненты/обновления, убрать дубли и сохранить результат
// в JSON-файл, который потом читает Unity.
// ============================================================



using System;                          // базовые типы (DateTime, и т.д.)
using System.Collections.Generic;      // List<T>, Dictionary<K,V>
using System.IO;                       // работа с файлами и папками
using System.Text.Json;                // сериализация в JSON (встроена в .NET)
using System.Text.Json.Serialization;  // доп. настройки сериализации (JsonIgnoreCondition)
using Console_Tracker.Models;          // ваши классы ProgramInfo / ProgramCatalog
using Microsoft.Win32;                 // доступ к реестру Windows

namespace Console_Tracker.Helpers
{
    public static class ProgramScanner
    {
        // Куда пишем файл — общая для обоих приложений папка.
        // ВАЖНО: в Unity-скрипте путь должен вычисляться так же.
        public static string GetOutputPath()
        {
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(baseDir, "TimeTracker");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "programs.json");
        }

        public static ProgramCatalog Scan()
        {
            var result = new Dictionary<string, ProgramInfo>(StringComparer.OrdinalIgnoreCase);

            ReadRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", result);
            ReadRegistry(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", result);
            ReadRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", result);

            return new ProgramCatalog
            {
                GeneratedAt = DateTime.UtcNow,
                Items = new List<ProgramInfo>(result.Values)
            };
        }

        private static void ReadRegistry(RegistryKey root, string path, Dictionary<string, ProgramInfo> result)
        {
            using RegistryKey uninstall = root.OpenSubKey(path);
            if (uninstall == null) return;

            foreach (string subKeyName in uninstall.GetSubKeyNames())
            {
                using RegistryKey key = uninstall.OpenSubKey(subKeyName);
                if (key == null) continue;

                string displayName = key.GetValue("DisplayName") as string;
                if (string.IsNullOrWhiteSpace(displayName)) continue;

                // Отсекаем системные компоненты (не отдельные приложения)
                if (key.GetValue("SystemComponent") is int sysComp && sysComp == 1) continue;

                // Отсекаем обновления/хотфиксы, если помечены явно
                string releaseType = key.GetValue("ReleaseType") as string;
                if (releaseType is "Update" or "Hotfix" or "ServicePack" or "Security Update") continue;

                // У большинства реальных приложений есть UninstallString
                string uninstallString = key.GetValue("UninstallString") as string;
                string quietUninstall = key.GetValue("QuietUninstallString") as string;
                if (string.IsNullOrEmpty(uninstallString) && string.IsNullOrEmpty(quietUninstall)) continue;

                var info = new ProgramInfo
                {
                    Name = displayName,
                    Version = key.GetValue("DisplayVersion") as string,
                    Publisher = key.GetValue("Publisher") as string,
                    InstallLocation = key.GetValue("InstallLocation") as string,
                    IconPath = key.GetValue("DisplayIcon") as string,
                    InstallDate = key.GetValue("InstallDate") as string
                };

                // Дедупликация по имени: если уже есть — оставляем запись
                // с более полными данными (например, с непустым InstallLocation)
                if (!result.TryGetValue(info.Name, out var existing) ||
                    (string.IsNullOrEmpty(existing.InstallLocation) && !string.IsNullOrEmpty(info.InstallLocation)))
                {
                    result[info.Name] = info;
                }
            }
        }

        // Атомарная запись: сначала во временный файл, потом переименование.
        // Так Unity никогда не прочитает файл в недописанном виде.
        public static void SaveToFile(ProgramCatalog catalog, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, JsonSerializer.Serialize(catalog, options));
            File.Copy(tempPath, path, overwrite: true);
            File.Delete(tempPath);
        }

        // Точка входа для использования в Main() консольного приложения
        public static void ScanAndSave()
        {
            var catalog = Scan();
            SaveToFile(catalog, GetOutputPath());
        }
    }
}
