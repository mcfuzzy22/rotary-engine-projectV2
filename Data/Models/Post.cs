// In Data/Models/Post.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rotaryproject.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PostRendition> Renditions { get; set; } = new List<PostRendition>();
    }
}