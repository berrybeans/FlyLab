using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlyLab.Models
{
    public class LabAreaViewModel
    {
        public Module Module { get; set; }
        public UseInstance Instance { get; set; }
        public ImageSettings[] ImageLib { get; set; }
        public string ImageGuide { get; set; }
        public string CatTemplate { get; set; }
        public bool debug { get; set; }
        public bool dev { get; set;}
    }
}