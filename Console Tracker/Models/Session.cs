using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Models
{
    public class Session
    {
      
            public string ProcessName { get; set; }

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public int DurationSeconds { get; set; }

            public Session()
            {
            }
        
    }
}
