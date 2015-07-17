//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlyLab.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fly
    {
        public Fly()
        {
            this.Traits = new HashSet<Trait>();
        }
    
        public int Id { get; set; }
        public int GenderId { get; set; }
        public int ModuleId { get; set; }
        public int UseInstanceId { get; set; }
    
        public virtual Gender Gender { get; set; }
        public virtual Module Module { get; set; }
        public virtual ICollection<Trait> Traits { get; set; }
        public virtual UseInstance UseInstance { get; set; }
    }
}
