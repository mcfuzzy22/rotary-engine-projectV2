using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rotaryproject.Data.Models;

[Index("Name", Name = "UQ__PartCate__737584F67434BF54", IsUnique = true)]
public partial class PartCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    // This is the foreign key for the parent category
    public int? ParentCategoryId { get; set; }

    // Navigation property to the parent category
    [ForeignKey("ParentCategoryId")]
        public virtual PartCategory? ParentCategory { get; set; }

    // Navigation property to the sub-categories
    public virtual ICollection<PartCategory> SubCategories { get; set; } = new List<PartCategory>();

    // The level of the category in the hierarchy
    public int CategoryLevel { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}