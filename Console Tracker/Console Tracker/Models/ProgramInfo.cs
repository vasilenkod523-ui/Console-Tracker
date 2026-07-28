using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Models
{
    // Модель одной программы. Поля можно расширять при необходимости.
    public class ProgramInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string InstallLocation { get; set; }
        public string IconPath { get; set; }
        public string InstallDate { get; set; } // формат из реестра: YYYYMMDD
    }
}
