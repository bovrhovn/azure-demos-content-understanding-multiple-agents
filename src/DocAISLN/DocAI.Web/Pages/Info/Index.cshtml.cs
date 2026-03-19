using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocAI.Web.Pages.Info;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet() => 
        logger.LogInformation("Index page visited at {Time}", DateTime.UtcNow);
}