using Topic.QueryService.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddQueryServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello Topic.QueryService");
app.UseApiServices();

app.Run();
