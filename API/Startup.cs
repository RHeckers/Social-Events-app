﻿using System.Text;
using API.Middleware;
using Application.Activities;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add database context and configure options
            services.AddDbContext<DataContext>(opt => {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            // Set cors policies
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                });
            });
            // Add mediator 
            services.AddMediatR(typeof(List.Handler).Assembly);

            // 
            services.AddAutoMapper(typeof(List.Handler));

            // Add MVC
            services.AddMvc(opt => {
                // Create Auth policy for all requests
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                // Add Auth policy
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Configure Identity
            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            // Add custom request validator IsHost
            services.AddAuthorization(opt => {
                opt.AddPolicy("IsActivityHost", policy => {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            // Generate key and add authentication as a service
            // TokenKey only avalible in dev mode
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            // Add IJWT as service with concrete instance of JwtGenerator
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            // Add IUserAccessor as a service with concrete instance of UserAccessor
            services.AddScoped<IUserAccessor, UserAccessor>();

            // Add photo IPhotoAccessor as a service with concrete instance of PhotoAccessor
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            // Add Cloudinary configuration, strongly type to the Cloudinary section of user-secrets
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();

            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // Use services in the app configuration -- order is important
            // app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
