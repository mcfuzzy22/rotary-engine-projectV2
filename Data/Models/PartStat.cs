using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rotaryproject.Data.Models;

[Index("PartId", Name = "IX_PartStats_PartID")]
public partial class PartStat
{
    [Key]
    [Column("PartStatID")]
    public int PartStatId { get; set; }

    [Column("PartID")]
    public int PartId { get; set; }

    [StringLength(100)]
    public string StatName { get; set; } = null!;

    [StringLength(100)]
    public string StatValue { get; set; } = null!;

    [StringLength(20)]
    public string? Unit { get; set; }

    [ForeignKey("PartId")]
    [InverseProperty("PartStats")]
    public virtual Part Part { get; set; } = null!;
}
