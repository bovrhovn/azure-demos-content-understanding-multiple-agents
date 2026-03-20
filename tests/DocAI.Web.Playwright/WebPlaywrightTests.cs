using Microsoft.Playwright.NUnit;

namespace DocAI.Web.Playwright;

/// <summary>
/// End-to-end browser tests for the DocAI.Web application using Playwright.
/// </summary>
[TestFixture]
[Category("E2E")]
public class HomePageTests : PageTest
{
    private string BaseUrl => DocAiServerSetup.BaseUrl;

    [Test]
    public async Task HomePage_LoadsSuccessfully()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("DocAI"));
    }

    [Test]
    public async Task HomePage_ContainsNavbar()
    {
        await Page.GotoAsync(BaseUrl);

        var navbar = Page.Locator("nav.docai-navbar");
        await Expect(navbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_ContainsBrandLink()
    {
        await Page.GotoAsync(BaseUrl);

        var brand = Page.Locator(".navbar-brand");
        await Expect(brand).ToBeVisibleAsync();
        await Expect(brand).ToContainTextAsync("DocAI");
    }

    [Test]
    public async Task HomePage_ContainsHeroSection()
    {
        await Page.GotoAsync(BaseUrl);

        var hero = Page.Locator("section.docai-hero");
        await Expect(hero).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_ContainsGitHubLink()
    {
        await Page.GotoAsync(BaseUrl);

        var githubLink = Page.Locator("a[href*='github.com']").First;
        await Expect(githubLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_FooterIsPresent()
    {
        await Page.GotoAsync(BaseUrl);

        var footer = Page.Locator("footer.docai-footer");
        await Expect(footer).ToBeVisibleAsync();
    }
}

/// <summary>
/// End-to-end browser tests for the Privacy page.
/// </summary>
[TestFixture]
[Category("E2E")]
public class PrivacyPageTests : PageTest
{
    private string BaseUrl => DocAiServerSetup.BaseUrl;

    [Test]
    public async Task PrivacyPage_LoadsSuccessfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Info/Privacy");

        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Privacy"));
    }

    [Test]
    public async Task PrivacyPage_ContainsPrivacyContent()
    {
        await Page.GotoAsync($"{BaseUrl}/Info/Privacy");

        var heading = Page.Locator("h1");
        await Expect(heading).ToContainTextAsync("Privacy");
    }

    [Test]
    public async Task PrivacyPage_NavigationLinksWork()
    {
        await Page.GotoAsync($"{BaseUrl}/Info/Privacy");

        // Click the Home link in the navbar
        await Page.Locator("nav a[href='/']").First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/"));
    }
}

/// <summary>
/// End-to-end browser tests for Upload and Process pages with new side-by-side layout.
/// </summary>
[TestFixture]
[Category("E2E")]
public class UploadPageLayoutTests : PageTest
{
    private string BaseUrl => DocAiServerSetup.BaseUrl;

    [Test]
    public async Task UploadAndProcess_PageLoadsSuccessfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");

        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Upload and Process"));
    }

    [Test]
    public async Task UploadAndProcess_HasSideBySideLayout()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");

        // Check that the top row with side-by-side layout exists
        var topRow = Page.Locator(".row.g-4.mb-4").First;
        await Expect(topRow).ToBeVisibleAsync();

        // Verify upload card is in col-lg-6
        var uploadCard = topRow.Locator(".col-lg-6").First;
        await Expect(uploadCard).ToBeVisibleAsync();
        await Expect(uploadCard.Locator("h5")).ToContainTextAsync("Upload New File");

        // Verify file list card is in col-lg-6
        var fileListCard = topRow.Locator(".col-lg-6").Last;
        await Expect(fileListCard).ToBeVisibleAsync();
        await Expect(fileListCard.Locator("h5")).ToContainTextAsync("Uploaded Files");
    }

    [Test]
    public async Task UploadAndProcess_UploadCardContainsDropZone()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");

        var dropZone = Page.Locator("#dropZone");
        await Expect(dropZone).ToBeVisibleAsync();
        await Expect(dropZone).ToContainTextAsync("Drop PDF here");
    }

    [Test]
    public async Task UploadAndProcess_ProcessButtonIsDisabledByDefault()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");

        var processBtn = Page.Locator("#processSelectedBtn");
        
        // Check if button exists (only if files are present)
        var btnCount = await processBtn.CountAsync();
        if (btnCount > 0)
        {
            await Expect(processBtn).ToBeDisabledAsync();
        }
    }

    [Test]
    public async Task UploadAndProcessWithADI_PageLoadsSuccessfully()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcessWithADI");

        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Azure Document Intelligence"));
    }

    [Test]
    public async Task UploadAndProcessWithADI_HasSideBySideLayout()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcessWithADI");

        // Check that the top row with side-by-side layout exists
        var topRow = Page.Locator(".row.g-4.mb-4").First;
        await Expect(topRow).ToBeVisibleAsync();

        // Verify upload card is in col-lg-6
        var uploadCard = topRow.Locator(".col-lg-6").First;
        await Expect(uploadCard).ToBeVisibleAsync();
        await Expect(uploadCard.Locator("h5")).ToContainTextAsync("Upload New File");

        // Verify file list card is in col-lg-6
        var fileListCard = topRow.Locator(".col-lg-6").Last;
        await Expect(fileListCard).ToBeVisibleAsync();
        await Expect(fileListCard.Locator("h5")).ToContainTextAsync("Uploaded Files");
    }

    [Test]
    public async Task UploadAndProcessWithADI_ProgressStepsArePresent()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcessWithADI");

        // The progress indicator should exist but be hidden by default
        var progressIndicator = Page.Locator("#processingIndicator");
        await Expect(progressIndicator).ToBeAttachedAsync();
        
        // Check for progress steps structure
        var progressSteps = progressIndicator.Locator(".docai-progress-steps");
        await Expect(progressSteps).ToBeAttachedAsync();
    }

    [Test]
    public async Task UploadPages_FooterIsStickyAtBottom()
    {
        await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");

        var footer = Page.Locator("footer.docai-footer");
        await Expect(footer).ToBeVisibleAsync();

        // Check that body uses flexbox for sticky footer
        var body = Page.Locator("body");
        var displayStyle = await body.EvaluateAsync<string>("el => window.getComputedStyle(el).display");
        Assert.That(displayStyle, Is.EqualTo("flex"), "Body should use flex display for sticky footer");
    }
}

/// <summary>
/// Accessibility and responsive layout tests.
/// </summary>
[TestFixture]
[Category("E2E")]
public class LayoutTests : PageTest
{
    private string BaseUrl => DocAiServerSetup.BaseUrl;

    [Test]
    public async Task Layout_FontAwesomeIconsAreLoaded()
    {
        await Page.GotoAsync(BaseUrl);

        // Check that Font Awesome stylesheet link is present
        var faLink = Page.Locator("link[href*='font-awesome']");
        var count = await faLink.CountAsync();
        Assert.That(count, Is.GreaterThan(0), "Font Awesome stylesheet should be linked");
    }

    [Test]
    public async Task Layout_BootstrapIsLoaded()
    {
        await Page.GotoAsync(BaseUrl);

        // Check that Bootstrap stylesheet link is present
        var bsLink = Page.Locator("link[href*='bootstrap']");
        var count = await bsLink.CountAsync();
        Assert.That(count, Is.GreaterThan(0), "Bootstrap stylesheet should be linked");
    }

    [Test]
    public async Task Layout_MobileMenuTogglerIsPresent()
    {
        await Page.GotoAsync(BaseUrl);

        var toggler = Page.Locator("button.navbar-toggler");
        await Expect(toggler).ToBeAttachedAsync();
    }
}
