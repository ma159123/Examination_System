using Microsoft.OpenApi;
using System.Reflection;
using System.Threading.RateLimiting;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
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
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Examination_System",
                Description = "An ASP.NET Core Web API for managing Diplomas."
            });

            options.UseInlineDefinitionsForEnums();
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //      options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: eyJhbGci..."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });



        return services;
    }
}
