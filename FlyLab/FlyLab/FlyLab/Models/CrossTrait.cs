using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlyLab.Models
{
    public class CrossTrait
    {
        private bool male;
        private bool female;
        private bool distance_based;
        private double rate;
        private int used;
        private Trait trait;
        private List<Trait> dist_traits;

        public CrossTrait(Trait p_trait, double p_rate, bool m = true, bool f = true)
        {
            this.rate = p_rate;
            this.trait = p_trait;
            this.dist_traits = null;
            this.male = m;
            this.female = f;
            this.used = 0;
            this.distance_based = false;
        }

        public CrossTrait(List<Trait> p_traits, double p_rate, bool m = true, bool f = true)
        {
            this.rate = p_rate;
            this.trait = p_traits.ElementAt(0);
            this.dist_traits = p_traits;
            this.male = m;
            this.female = f;
            this.used = 0;
            this.distance_based = true;
        }

        public double Rate
        {
            get { return this.rate; }
            set { this.rate = value; }
        }

        public Trait Trait
        {
            get { return this.trait; }
            set { this.trait = value; }
        }

        public List<Trait> DistanceCrossedTraits
        {
            get { return this.dist_traits; }
            set { this.dist_traits = value; }
        }

        public int Used
        {
            get { return this.used; }
            set { this.used = value; }
        }

        public bool Male
        {
            get { return this.male; }
            set { this.male = value; }
        }

        public bool Female
        {
            get { return this.female; }
            set { this.female = value; }
        }

        public bool DistanceBased
        {
            get { return this.distance_based; }
            set { this.distance_based = value; }
        }

    }
}