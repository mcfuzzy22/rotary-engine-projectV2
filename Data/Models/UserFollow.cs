// In Data/Models/UserFollow.cs
namespace rotaryproject.Data.Models
{
    public class UserFollow
    {
        // Composite Primary Key is configured in DbContext
        public string FollowerId { get; set; }
        public virtual ApplicationUser Follower { get; set; }

        public string FollowingId { get; set; }
        public virtual ApplicationUser Following { get; set; }
    }
}