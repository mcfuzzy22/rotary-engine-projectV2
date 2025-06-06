using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rotaryproject.Data.Models;

[Index("PartAId", Name = "IX_CompatibilityRules_PartA_ID")]
[Index("PartBId", Name = "IX_CompatibilityRules_PartB_ID")]
[Index("PartAId", "PartBId", Name = "UQ__Compatib__C9D6D737D786A5C9", IsUnique = true)]
public partial class CompatibilityRule
{
    [Key]
    [Column("RuleID")]
    public int RuleId { get; set; }

    [Column("PartA_ID")]
    public int PartAId { get; set; }

    [Column("PartB_ID")]
    public int PartBId { get; set; }

    public bool? IsCompatible { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("PartAId")]
    [InverseProperty("CompatibilityRulePartAs")]
    public virtual Part PartA { get; set; } = null!;

    [ForeignKey("PartBId")]
    [InverseProperty("CompatibilityRulePartBs")]
    public virtual Part PartB { get; set; } = null!;
}
