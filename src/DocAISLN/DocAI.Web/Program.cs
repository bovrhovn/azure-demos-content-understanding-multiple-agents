var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(settings => settings.AddServerHeader = false);
builder.Services.AddTransient<ILogger>(p =>
{
    var loggerFactory = p.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger("frontend");
});
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();// Expose Program class for WebApplicationFactory in test projects
public partial class Program { }
