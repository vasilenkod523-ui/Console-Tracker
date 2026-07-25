using Microsoft.Win32;
using System.Collections.Generic;
using UnityEngine;


public class VuiwProcesses : MonoBehaviour
{
    // Список найденных названий программ
    public List<string> programs = new List<string>();


    void Start()
    {
        // Получаем список программ при запуске
        ScanPrograms();


        // Выводим найденные программы в Console Unity
        foreach (string program in programs)
        {
            Debug.Log(program);
        }
    }



    // Запускает сканирование двух разделов реестра Windows
    void ScanPrograms()
    {
        programs.Clear();


        // Программы Windows 64-bit
        ReadRegistry(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
        );


        // Программы Windows 32-bit
        ReadRegistry(
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        );
    }



    // Читает один раздел реестра и получает названия программ
    void ReadRegistry(string path)
    {
        Microsoft.Win32.RegistryKey uninstall =
            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);


        if (uninstall == null)
            return;



        foreach (string programName in uninstall.GetSubKeyNames())
        {
            Microsoft.Win32.RegistryKey program =
                uninstall.OpenSubKey(programName);


            if (program == null)
                continue;


            string displayName =
                program.GetValue("DisplayName") as string;



            if (!string.IsNullOrEmpty(displayName))
            {
                programs.Add(displayName);
            }


            program.Close();
        }


        uninstall.Close();
    }
}