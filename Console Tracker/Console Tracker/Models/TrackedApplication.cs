using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Models
{
    public class TrackedApplication
    {

        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
