using Application;
using HotelManagement.Application.Mapping;
using Infrastructure;
using System.Threading.RateLimiting;

namespace Examination_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 1. إضافة خدمة الـ Rate Limiter
            builder.Services.AddRateLimiter(options =>
            {

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                    {
                        context.HttpContext.Response.Headers["Retry-After"] = ((int)retryAfter.TotalSeconds).ToString();
                    }

                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        "{\"success\": false, \"message\": \"Too many requests. Please try again later.\", \"data\": null, \"error\": \"Rate limit exceeded\", \"meta\": null}",
                        cancellationToken: token);
                };

                // 2. تعيين Policy مخصصة للـ Auth Endpoints (10 طلبات / دقيقة لكل IP)
                options.AddPolicy("AuthRateLimit", httpContext =>
                {
                    // الحصول على الـ IP الخاص بالعميل
                    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: clientIp,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,                            // 10 requests
                            Window = TimeSpan.FromMinutes(1),            // per 1 minute
                            QueueLimit = 0,                              // لا تسمح بانتظار الطلبات الزائدة
                            AutoReplenishment = true
                        });
                });
            });
            builder.Services.AddControllers();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddAppDependencies();
            builder.Services.AddMappings();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


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

            app.UseAuthorization();

            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }
    }
}
