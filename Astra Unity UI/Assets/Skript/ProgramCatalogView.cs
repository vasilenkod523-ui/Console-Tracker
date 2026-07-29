// ============================================================
// ProgramCatalogView.cs — часть UNITY-приложения
//
// Что делает этот файл:
// 1. Находит JSON-файл, который сохранило консольное приложение
// 2. Читает его
// 3. Превращает JSON обратно в список программ
//
// Никаких дополнительных пакетов ставить не нужно —
// используется встроенный в Unity JsonUtility.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Важно: названия полей здесь должны совпадать с названиями
// в JSON-файле (то есть с полями ProgramInfo в консольном приложении)
[Serializable]
public class ProgramInfo
{
    public string Name;
    public string Version;
    public string Publisher;
    public string InstallLocation;
}

[Serializable]
public class ProgramCatalog
{
    public string GeneratedAt;
    public List<ProgramInfo> Items;
}

public class ProgramCatalogView : MonoBehaviour
{
    // Сюда попадёт список программ после загрузки
    public List<ProgramInfo> programs = new List<ProgramInfo>();

    void Start()
    {
        LoadPrograms();
    }

    // Читает JSON-файл и заполняет список programs
    public void LoadPrograms()
    {
        string filePath = GetFilePath();

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Файл не найден: " + filePath +
                "\nСначала запустите консольное приложение, чтобы он появился.");
            return;
        }

        string json = File.ReadAllText(filePath);

        ProgramCatalog catalog = JsonUtility.FromJson<ProgramCatalog>(json);
        programs = catalog.Items;

        foreach (ProgramInfo p in programs)
        {
            Debug.Log(p.Name + " — " + p.Version);
        }
    }

    // Путь должен быть таким же, как в консольном приложении
    string GetFilePath()
    {
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(folder, "TimeTracker", "programs.json");
    }
}
