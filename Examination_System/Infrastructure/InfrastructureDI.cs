using Application.Interfaces;
using Domain.Entites;
using Infrastructure.Data;
using Infrastructure.InterfacesImpl;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            return services;
        }
    }
}
