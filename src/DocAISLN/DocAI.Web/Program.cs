using DocAI.Services;
using DocAI.Services.Data;
using DocAI.Services.General;
using DocAI.Services.Options;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(settings =>
{
    settings.AddServerHeader = false;
    settings.Limits.MaxRequestBodySize = 6L * 1024 * 1024 * 1024; // 6 GB
    settings.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    settings.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddTransient<ILogger>(p =>
{
    var loggerFactory = p.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger("frontend");
});
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register PDF and storage services
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<LocalStorageService>();

// Register document processing services
builder.Services.Configure<DocumentProcessingOptions>(options =>
{
    options.FoundryEndpointMain = builder.Configuration["AzureAI:FoundryEndpointMain"] 
        ?? Environment.GetEnvironmentVariable("FOUNDRYENDPOINTMAIN") ?? string.Empty;
    options.FoundryEndpointMini = builder.Configuration["AzureAI:FoundryEndpointMini"] 
        ?? Environment.GetEnvironmentVariable("FOUNDRYENDPOINTMINI") ?? string.Empty;
    options.MainModelName = builder.Configuration["AzureAI:MainModelName"] 
        ?? Environment.GetEnvironmentVariable("ORCHESTRATORMODEL") ?? string.Empty;
    options.MiniModelName = builder.Configuration["AzureAI:MiniModelName"] 
        ?? Environment.GetEnvironmentVariable("MINIMODEL") ?? string.Empty;
});

builder.Services.Configure<AzureDocIntelligenceOptions>(options =>
{
    options.DocumentEndpoint = builder.Configuration["AzureAI:DocumentEndpoint"] 
        ?? Environment.GetEnvironmentVariable("DOCENDPOINT") ?? string.Empty;
    options.ModelId = builder.Configuration["AzureAI:DocumentModelId"] 
        ?? Environment.GetEnvironmentVariable("DOCMODELID") ?? "prebuilt-layout";
});

builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
builder.Services.AddScoped<IAzureDocIntelligenceService, AzureDocIntelligenceService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Info/Error");
}

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();

// Expose Program class for WebApplicationFactory in test projects
namespace DocAI.Web
{
    public partial class Program { }
}
