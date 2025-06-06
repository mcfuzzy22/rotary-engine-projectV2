// In Data/Models/UserSavedBuild.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotaryproject.Data.Models // Ensure this namespace is correct
{
    public class UserSavedBuild
    {
        [Key]
        public int UserSavedBuildId { get; set; }

        [Required]
        public string BuildName { get; set; } = "My Engine Build"; // User-defined name for the build

        [Required]
        public string UserId { get; set; } = string.Empty; // Foreign key to the ApplicationUser (AspNetUsers table)

        [Required]
        public string BuildConfigurationString { get; set; } = string.Empty; // e.g., "4-4_1-101"

        public DateTime SavedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        // Navigation property to the user
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}