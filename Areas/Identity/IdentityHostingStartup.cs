using System;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(EeshaProperty.Areas.Identity.IdentityHostingStartup))]
namespace EeshaProperty.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<ApplicationUser>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                //     {
                //         options.SignIn.RequireConfirmedAccount = true;
                //         options.Password.RequireLowercase = true;
                //         options.Password.RequireUppercase = true;
                //         options.Password.RequireDigit = true;
                //         options.Password.RequiredLength = 8;                       
                //     })
                //     .AddEntityFrameworkStores<ApplicationDbContext>()
                //     .AddDefaultTokenProviders();

                services.AddMvc(options =>
                    {
                        var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    }).AddXmlSerializerFormatters();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Admin", policy =>
                    policy.RequireUserName("admin"));
                });

            });
        }
    }
}