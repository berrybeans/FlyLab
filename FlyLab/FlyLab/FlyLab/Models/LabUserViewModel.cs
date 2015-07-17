using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlyLab.Models
{
    public class LabUserViewModel
    {
        public LabUser User { get; set; }
        public Module Module { get; set; }
        public UseInstance lastInstance { get; set; }
        public string LastStart { get; set; }
        public string LastFinish { get; set; }
        public int LabsCompleted { get; set; }
    }
}