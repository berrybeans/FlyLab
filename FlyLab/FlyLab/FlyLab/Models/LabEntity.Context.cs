﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LabEntityContainer : DbContext
    {
        public LabEntityContainer()
            : base("name=LabEntityContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Fly> Fly { get; set; }
        public DbSet<Trait> Trait { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<LabUser> LabUser { get; set; }
        public DbSet<UseInstance> UseInstance { get; set; }
        public DbSet<ImageSettings> ImageSettings { get; set; }
    }
}