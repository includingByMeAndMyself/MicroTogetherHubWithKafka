using Topic.CommandService.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommandServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();
