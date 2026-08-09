using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Tracker.Models
{
    public class ProgramCatalog
    {
        public DateTime GeneratedAt { get; set; }
        public List<ProgramInfo> Items { get; set; } = new List<ProgramInfo>();
    }
}
