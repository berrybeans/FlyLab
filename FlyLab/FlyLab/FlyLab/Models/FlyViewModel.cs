using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlyLab.Models;

namespace FlyLab.Models
{
    public class FlyViewModel
    {
        public int Frequency { get; set; }
        public int Expected { get; set; }
        public int FlyID { get; set; }
        public string Gender { get; set; }
        public ReducedTraitModel[] Traits { get; set; }
    }
}