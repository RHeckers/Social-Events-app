using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }

        // Virtual to lazy load the related data incombination with UseLazyLoadingProxies
        public virtual ICollection<UserActivity> UserActivities { get; set; }
    }
}