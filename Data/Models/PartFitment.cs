using System.ComponentModel.DataAnnotations.Schema;
using rotaryproject.Data;
using rotaryproject.Data.Models;
namespace rotaryproject.Data.Models
{
    /// <summary>
    /// This is the linking table that defines which EngineFamilies a Part can fit.
    /// A single row creates one valid "fitment" for a part.
    /// </summary>
    public class PartFitment
    {
        // Composite Primary Key is configured in the DbContext
        
        public int PartId { get; set; }
        public int EngineFamilyId { get; set; }

        [ForeignKey("PartId")]
        public virtual Part Part { get; set; } = null!;

        [ForeignKey("EngineFamilyId")]
        public virtual EngineFamily EngineFamily { get; set; } = null!;
    }
}