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
    
    public partial class LabUser
    {
        public LabUser()
        {
            this.UseInstances = new HashSet<UseInstance>();
        }
    
        public int Id { get; set; }
        public string GID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<UseInstance> UseInstances { get; set; }
    }
}
