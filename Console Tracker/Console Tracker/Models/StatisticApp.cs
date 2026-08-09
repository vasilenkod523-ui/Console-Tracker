using Console_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Console_Tracker.Models
{
    public class StatisticApp
    {
        public string ProcessName { get; set; }
        public List<TimeSpan> UsageTimes { get; set; }
    }
}


