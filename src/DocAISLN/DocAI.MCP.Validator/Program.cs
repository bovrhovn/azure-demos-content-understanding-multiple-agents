var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(settings => settings.AddServerHeader = false);
builder.Services.AddTransient<ILogger>(p =>
{
    var loggerFactory = p.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger("mcpvalidator");
});
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();