using API;
using Application;
using HotelManagement.Application.Mapping;
using Infrastructure;

namespace Examination_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 1. إضافة خدمة الـ Rate Limiter

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddAppDependencies();
            builder.Services.AddMappings();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddPresentation();

            var app = builder.Build();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
            //    var userManager = services.GetRequiredService<UserManager<AppUser>>();

            //    string adminEmail = "mae562190@gmail.cpm";

            //    await IdentitySeeder.SeedRolesAndAdminAsync(roleManager, userManager, adminEmail);
            //}

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.Value?.StartsWith("/api/auth") ?? false)
                    {
                        context.Response.Headers["X-RateLimit-Limit"] = "10";
                    }
                    await next();
                });
            }

            app.UseHttpsRedirection();


            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
