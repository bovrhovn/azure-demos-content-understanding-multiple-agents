using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocAI.Web.Pages.Docs;

public class UploadAndProcessPageModel(ILogger<UploadAndProcessPageModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("Accessed Upload and Process page at {DateLoaded}.", DateTime.Now);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Accessed Upload and Process page at {DateLoaded}.", DateTime.Now);
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var safeFileName = Path.GetRandomFileName(); 
        var filePath = Path.Combine(uploadsFolder, safeFileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            if (FormFile != null) 
                await FormFile.CopyToAsync(stream);
        }

        //TODO: agent calls to fill in the data 
        
        return RedirectToPage("/Docs/UploadAndProcess");
    }
    
    
    [BindProperty]
    public IFormFile? FormFile { get; set; }
}