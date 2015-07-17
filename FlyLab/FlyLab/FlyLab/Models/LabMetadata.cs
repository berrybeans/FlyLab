using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FlyLab.Models
{
    [MetadataType(typeof(TraitMetadata))]
    public partial class Trait { }
    public class TraitMetadata
    {
        [Required()]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage="Only letters are allowed in the name field.")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required()]
        [Display(Name="Dominant", ShortName="Dom.")]
        public bool IsDominant { get; set; }

        [Required()]
        [Display(Name="Inc. Dominant", ShortName="Inc. Dom.")]
        public bool IsIncompleteDominant { get; set; }

        [Required()]
        [Display(Name="Lethal")]
        public bool IsLethal { get; set; }

        [Required()]
        [RegularExpression(@"[0-9]+$", ErrorMessage="Only integers are allowed in the chromosome field.")]
        [Display(Name="Chromosome")]
        public byte ChromosomeNumber { get; set; }

        [Required()]
        [RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?", ErrorMessage = "The distance field must be a decimal number.")]
        public double Distance { get; set; }

        [Required()]
        [Display(Name="Image Path")]
        [MaxLength(20)]
        public string ImagePath { get; set; }
    }

    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category { }
    public class CategoryMetadata
    {
        [Display(Name = "Category Name")]
        [MaxLength(20)]
        public string CatName { get; set; }
    }

    [MetadataType(typeof(ModuleMetadata))]
    public partial class Module { }
    public class ModuleMetadata
    {
        [Display(Name = "Module Name")]
        [MaxLength(20)]
        public string ModuleName { get; set; }

        [Display(Name = "CallID")]
        public int Call_id { get; set; }
    }

    [MetadataType(typeof(ImageMetadata))]
    public partial class ImageSettings { }
    public class ImageMetadata
    {
        [Display(Name = "First Category")]
        public string FirstCat { get; set; }

        [Display(Name = "Second Category")]
        public string SecCat { get; set; }

        [Required()]
        [MaxLength(100)]
        public string Prefix { get; set; }

        [Required()]
        [MaxLength(100)]
        public string Suffix { get; set; }
    }
}