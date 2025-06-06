using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rotaryproject.Data;
namespace rotaryproject.Data.Models
{
    public partial class Part
    {
        [Key]
        [Column("PartID")]
        public int PartId { get; set; } // Ensure casing matches your DB/usage

        [Required]
        public int CategoryId { get; set; } // Ensure casing matches your DB/usage

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [ForeignKey("CategoryId")]
        [InverseProperty("Parts")] // This points to the ICollection<Part> in PartCategory.cs
        public virtual PartCategory Category { get; set; } = null!;

        // NEW: Navigation property for the many-to-many relationship
        public virtual ICollection<PartFitment> Fitments { get; set; } = new List<PartFitment>();
        [StringLength(50)]
        public string? Sku { get; set; } // Ensure casing matches your DB/usage

        public string? Description { get; set; }


        [StringLength(255)]
        public string? ImagePath { get; set; } // For 2D thumbnail image

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? BasePrice { get; set; }

        // New properties for detailed information (some might move to PartStats or related tables later)
        [StringLength(100)]
        public string? Brand { get; set; }
         [StringLength(100)]
        public string? Availability { get; set; } // e.g., "In Stock", "Backorder", "Ships in 2-3 days"

        [StringLength(100)]
        public string? VendorName { get; set; } // e.g., "MAZDATRIX", "Amazon", "Rotary Performance"

        [StringLength(512)] // URL can be long
        public string? VendorProductUrl { get; set; } // Direct link to buy the product

        [StringLength(100)]
        public string? Material { get; set; }

        public int? PieceCount { get; set; } // e.g., for seals

        [StringLength(50)]
        public string? SizeMm { get; set; } // e.g., "2mm", "3mm"

        public int? ManufacturingYear { get; set; }

        public int? SealAmount { get; set; } // How many seals are in this part/package

        public double? Rating { get; set; } // e.g., 0-5 stars
        public int? RatingCount { get; set; } // Number of reviews


        // Navigation property back to the catego

        // Navigation property to PartStats (if you use this for some attributes)
    }
}