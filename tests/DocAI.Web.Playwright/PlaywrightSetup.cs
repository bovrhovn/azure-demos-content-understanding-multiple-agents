using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DocAI.Web.Playwright;

/// <summary>
/// NUnit SetUpFixture that starts a real Kestrel instance of DocAI.Web
/// so Playwright can drive it through a real browser.
/// </summary>
[SetUpFixture]
public class DocAiServerSetup
{
    private WebApplication? _app;

    /// <summary>Gets the base URL of the running test server.</summary>
    public static string BaseUrl { get; private set; } = "";

    [OneTimeSetUp]
    public async Task SetUpAsync()
    {
        var webProjectDir = FindWebProjectDirectory();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            // ApplicationName must match the project that owns the compiled Razor views.
            ApplicationName = "DocAI.Web",
            // Point at the source project so wwwroot static files are resolved.
            ContentRootPath = webProjectDir,
            WebRootPath     = Path.Combine(webProjectDir, "wwwroot"),
        });

        // Remove the Server response header and listen on an OS-assigned port.
        builder.Services.Configure<KestrelServerOptions>(opts => opts.AddServerHeader = false);
        builder.WebHost.UseSetting("urls", "http://127.0.0.1:0");

        builder.Services.AddTransient<ILogger>(p =>
        {
            var factory = p.GetRequiredService<ILoggerFactory>();
            return factory.CreateLogger("frontend-test");
        });
        builder.Services.AddRazorPages().AddRazorPagesOptions(opts =>
            opts.Conventions.AddPageRoute("/Info/Index", ""));

        _app = builder.Build();
        _app.UseRouting();
        _app.UseAuthorization();
        _app.UseStaticFiles();
        _app.MapRazorPages();

        await _app.StartAsync();

        // Resolve the actual bound address (port 0 → OS-assigned port).
        var server    = _app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()!;
        BaseUrl = addresses.Addresses.First().TrimEnd('/');
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    /// <summary>
    /// Walks up from the test-binary directory to find the DocAI.Web source folder.
    /// Works both locally and in CI (where the binary sits several levels below the repo root).
    /// </summary>
    private static string FindWebProjectDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "DocAISLN", "DocAI.Web");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the DocAI.Web source directory. " +
            "Ensure the test binary is within the repository tree.");
    }
}
