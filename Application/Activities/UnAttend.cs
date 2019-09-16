using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UnAttend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // handler logic  
                var activity = await _context.Activities.FindAsync(request.Id);

                // If no activity throw rest exception
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Could not find Activity" });

                // Find the currently loggedin user
                var user = await _context.Users.SingleOrDefaultAsync(x =>
                                                 x.UserName == _userAccessor.GetCurrentUsername());

                // Check if the user is already attending the activity
                var attendance = await _context.UserActivities.SingleOrDefaultAsync(x =>
                                                            x.ActivityId == activity.Id
                                                            && x.AppUserId == user.Id);
                
                // If not attending return
                if (attendance == null)
                    return Unit.Value; 

                if (attendance.isHost)
                    throw new RestException(HttpStatusCode.BadRequest, new {Attendance = "Host cannot remove himself from the event"});

                _context.UserActivities.Remove(attendance);

                // Default succes error logic               
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}