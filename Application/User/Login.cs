using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<AppUser>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QuryValidator : AbstractValidator<Query>
        {
            public QuryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }


        public class Handler : IRequestHandler<Query, AppUser>
        {
            private readonly UserManager<AppUser> _usermanager;
            private readonly SignInManager<AppUser> _signinManager;
            public Handler(UserManager<AppUser> usermanager, SignInManager<AppUser> signinManager)
            {
                _signinManager = signinManager;
                _usermanager = usermanager;
            }

            public async Task<AppUser> Handle(Query request, CancellationToken cancellationToken)
            {
                // handler logic goes here
                var user = await _usermanager.FindByEmailAsync(request.Email);

                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);
                
                var result = await _signinManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
                {
                    // Generate token to send to user
                    return user;
                }

                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}