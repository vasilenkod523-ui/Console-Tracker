using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Models
{
    public class TrackerConfiguration
    {
        public List<TrackedApplication> Applications { get; set; } = new List<TrackedApplication>();

        public bool IsTrackingEnabled { get; set; }
    }
}
