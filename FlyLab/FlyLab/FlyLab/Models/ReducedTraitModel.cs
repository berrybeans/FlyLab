using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlyLab.Models
{
    public class ReducedTraitModel
    {
        public int Id { get; set; }
        public int ChromosomeNumber { get; set; }
        public int CategoryId { get; set; }
        public double Distance { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool IsHeterozygous { get; set; }
        public bool IsDominant { get; set; }
        public bool IsIncompleteDominant { get; set; }
        public bool IsLethal { get; set; }

        public ReducedTraitModel()
        {
            this.Id = 0;
        }

        public ReducedTraitModel(Trait t) {
            this.Id = t.Id;
            this.ChromosomeNumber = t.ChromosomeNumber;
            this.CategoryId = t.CategoryId;
            this.Distance = t.Distance;
            this.Name = t.Name;
            this.ImagePath = t.ImagePath;
            this.IsHeterozygous = t.IsHeterozygous ?? false;
            this.IsDominant = t.IsDominant;
            this.IsIncompleteDominant = t.IsIncompleteDominant;
            this.IsLethal = t.IsLethal;
        }
    }
}