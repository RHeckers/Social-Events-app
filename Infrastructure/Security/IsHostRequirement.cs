using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    // Custom reuqest validator 
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;

        // Constructor
        public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //Auth handler
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // Check if AuthorizationFilterContext
            if (context.Resource is AuthorizationFilterContext authContext)
            {
                // Find the username in the token
                var currentUserName = _httpContextAccessor.HttpContext.
                                      User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                
                // Get activity ID as a string from the API url
                var activityId = authContext.RouteData.Values["id"].ToString();

                // Find the activity in the Db, use result bechase not ASYNC
                var activity = _context.Activities.FindAsync(activityId).Result;

                // Check if the Activity has a UserActivity with host is true
                var host = activity.UserActivities.FirstOrDefault(x => x.isHost);

                // If the request sender is the host, then Auth succeeded
                if (host?.AppUser?.UserName == currentUserName)
                    context.Succeed(requirement);
            } else {
                  context.Fail();  
            }

            return Task.CompletedTask;
        }
    }
}