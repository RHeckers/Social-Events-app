using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // Query interface
        public class Query : IRequest<ActivityDto>
        {
            public Guid Id { get; set; }
        }

        // Mediator query handler class returning and ActivityDto
        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            // Constructor
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }

            // Query Handler logic
            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                // Get activity and include AppUser details and UserActivites details
                var activity = await _context.Activities
                                                .Include(x => x.UserActivities)
                                                .ThenInclude(x => x.AppUser)
                                                .SingleOrDefaultAsync(x => x.Id == request.Id);

                // If not found return error
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activitiy = "Not found" });

                // Map Activity to ActivityDto and return value to controller
                return _mapper.Map<Activity, ActivityDto>(activity);
            }
        }
    }
}