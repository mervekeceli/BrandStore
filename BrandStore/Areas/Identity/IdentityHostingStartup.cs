﻿using System;
using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BrandStore.Areas.Identity.IdentityHostingStartup))]
namespace BrandStore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ApplicationDbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                     .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                   .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }
    }
}