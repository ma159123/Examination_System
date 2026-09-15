using Application.Interfaces;
using Domain.Entites;
using Infrastructure.Data;
using Infrastructure.InterfacesImpl;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 1. أولاً: تسجيل Identity
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 2. ثانياً: تسجيل JWT Authentication (لتتغلب إعداداته على الـ Cookie Defaults الخاصة بـ Identity)
            services.AddJwtAuthentication(configuration);

            // Repositories & DbContext Services (Must be Scoped)
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IDiplomaRepo, DiplomaRepo>();

            // Application Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            // External Stateless Services (Can be Transient)
            services.AddTransient<IEmailService, EmailService>(); return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // إجبار النظام على استخدام JWT كخيار افتراضي مطلق لكل عمليات الـ Authenticate و Challenge
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Key is not configured.")))
                };
            });

            // إلغاء سلوك الـ Redirect كلياً للـ Cookies التي ينشئها Identity
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            return services;
        }
    }
}