using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using rotaryproject.Data;
using rotaryproject.Data.Models;
namespace rotaryproject.Data.Models
{
    /// <summary>
    /// Defines a distinct family of rotary engines, which serves as the
    /// primary group for compatibility checks. E.g., "S4 TII", "S5 NA", "13B-REW".
    /// </summary>
    public class EngineFamily
    {
        [Key]
        public int EngineFamilyId { get; set; }

        [Required]
        [StringLength(50)]
        public string FamilyCode { get; set; } = null!; // e.g., "S5-NA"

        [StringLength(255)]
        public string? Description { get; set; } // e.g., "From 1989-1992 Mazda RX-7 (Non-Turbo)"

        // Navigation property to the linking table
        public virtual ICollection<PartFitment> PartFitments { get; set; } = new List<PartFitment>();
    }
}