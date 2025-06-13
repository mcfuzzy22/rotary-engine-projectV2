// In Data/Models/PostRendition.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace rotaryproject.Data.Models
{
    public class PostRendition
    {
        [Key]
        public int PostRenditionId { get; set; }

        [Required]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        public string? Thoughts { get; set; }
        public string? ImageUrls { get; set; } // Can be a JSON string of URLs

        [Required]
        public string EngineBuildConfigurationString { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}