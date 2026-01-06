using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("YarpProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerClientPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromSeconds(30)
            }));

    options.OnRejected = (context, token) =>
    {
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
        logger?.LogWarning("Лимит запросов превышен для IP: {IP} | {Path}",
            ip, context.HttpContext.Request.Path);
        return ValueTask.CompletedTask;
    };
});

var app = builder.Build();

app.UseRateLimiter();
app.MapReverseProxy().RequireRateLimiting("PerClientPolicy");

app.Run();