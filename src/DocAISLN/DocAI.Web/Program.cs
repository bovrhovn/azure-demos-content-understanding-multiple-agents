using System.Net;
using DocAI.Services.Data;
using DocAI.Services.General;
using DocAI.Services.Options;
using DocAI.Web.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(settings =>
{
    settings.AddServerHeader = false;
    settings.Limits.MaxRequestBodySize = 6L * 1024 * 1024 * 1024; // 6 GB
    settings.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    settings.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddOptions<DocumentProcessingOptions>()
    .Bind(builder.Configuration.GetSection(DocumentProcessingOptions.SectionName))
    .ValidateDataAnnotations();
builder.Services.AddOptions<AzureDocIntelligenceOptions>()
    .Bind(builder.Configuration.GetSection(AzureDocIntelligenceOptions.SectionName))
    .ValidateDataAnnotations();
builder.Services.AddTransient<ILogger>(p =>
{
    var loggerFactory = p.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger("frontend");
});
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
// Register PDF and storage services
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<ProcessDataService>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
builder.Services.AddScoped<IAzureDocIntelligenceService, AzureDocIntelligenceService>();

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Info/Error");
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();
app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            var message = $"{exception.Error.Message}";
            await context.Response.WriteAsync(message);
        }
    });
});
app.MapHealthChecks($"/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();
app.MapRazorPages();
app.Run();

// Expose Program class for WebApplicationFactory in test projects
namespace DocAI.Web
{
    public partial class Program { }
}
