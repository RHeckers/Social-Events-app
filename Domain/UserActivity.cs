using System;

namespace Domain
{
    public class UserActivity
    {
        public string AppUserId { get; set; }

        // Virtual to lazy load the related data incombination with UseLazyLoadingProxies
        public virtual AppUser AppUser { get; set; }
        public Guid ActivityId { get; set; }

        // Virtual to lazy load the related data incombination with UseLazyLoadingProxies
        public virtual Activity Activity { get; set; }
        public DateTime DateJoined { get; set; }
        public bool isHost { get; set; }
    }
}