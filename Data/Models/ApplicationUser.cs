// In Data/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace rotaryproject.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custom properties for user profile
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }

        // Navigation properties
        public virtual ICollection<UserSavedBuild> SavedBuilds { get; set; } = new List<UserSavedBuild>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        
        // Follower/Following relationships
        public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public virtual ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    }
}