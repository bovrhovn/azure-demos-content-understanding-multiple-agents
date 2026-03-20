using System.Text.Json;
using DocAI.Models;
using DocAI.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocAI.Web.Pages.Docs;

public class UploadAndProcessWithADIPageModel(
    ILogger<UploadAndProcessWithADIPageModel> logger,
    IAzureDocIntelligenceService azureDocIntelligenceService) : PageModel
{
    [BindProperty]
    public IFormFile? FormFile { get; set; }
    
    public List<FileInfo> UploadedFiles { get; set; } = [];
    public ExtractedData? ProcessedData { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsProcessing { get; set; }

    public void OnGet()
    {
        logger.LogInformation("Accessed UploadAndProcessWithADI page at {DateLoaded}", DateTime.UtcNow);
        LoadUploadedFiles();
        LoadProcessedData();
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        logger.LogInformation("Upload requested at {DateLoaded}", DateTime.UtcNow);
        
        if (FormFile == null || FormFile.Length == 0)
        {
            ErrorMessage = "Please select a file to upload";
            LoadUploadedFiles();
            return Page();
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Use original file name with timestamp to avoid conflicts
        var fileName = $"{Path.GetFileNameWithoutExtension(FormFile.FileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(FormFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        try
        {
            await using (var stream = System.IO.File.Create(filePath))
            {
                await FormFile.CopyToAsync(stream);
            }

            logger.LogInformation("File uploaded successfully: {FileName}", fileName);
            TempData["SuccessMessage"] = $"File '{FormFile.FileName}' uploaded successfully!";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file: {FileName}", FormFile.FileName);
            ErrorMessage = "Error uploading file. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostProcessAsync(string fileName)
    {
        logger.LogInformation("ADI Process requested for file: {FileName}", fileName);

        if (string.IsNullOrEmpty(fileName))
        {
            ErrorMessage = "No file selected for processing";
            LoadUploadedFiles();
            return Page();
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            ErrorMessage = $"File '{fileName}' not found";
            LoadUploadedFiles();
            return Page();
        }

        try
        {
            IsProcessing = true;
            
            // Process the document using Azure Document Intelligence
            logger.LogInformation("Starting Azure Document Intelligence analysis for: {FileName}", fileName);
            var data = await azureDocIntelligenceService.AnalyzeDocumentAsync(filePath);
            
            // Store results in TempData
            TempData["ProcessedData"] = JsonSerializer.Serialize(data);
            TempData["ProcessedFileName"] = fileName;
            TempData["SuccessMessage"] = $"Document '{fileName}' analyzed successfully with Azure Document Intelligence!";
            
            logger.LogInformation("Document analyzed successfully: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing file with ADI: {FileName}", fileName);
            TempData["ErrorMessage"] = $"Error analyzing '{fileName}': {ex.Message}";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostClearResults()
    {
        TempData.Remove("ProcessedData");
        TempData.Remove("ProcessedFileName");
        TempData["SuccessMessage"] = "Results cleared";
        return RedirectToPage();
    }

    private void LoadUploadedFiles()
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        
        if (Directory.Exists(uploadsFolder))
        {
            var directoryInfo = new DirectoryInfo(uploadsFolder);
            UploadedFiles = directoryInfo.GetFiles("*.pdf")
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();
            
            logger.LogInformation("Found {FileCount} uploaded files", UploadedFiles.Count);
        }
    }

    private void LoadProcessedData()
    {
        if (TempData.ContainsKey("ErrorMessage"))
        {
            ErrorMessage = TempData["ErrorMessage"]?.ToString();
        }

        if (TempData.ContainsKey("ProcessedData"))
        {
            try
            {
                var json = TempData["ProcessedData"]?.ToString();
                if (!string.IsNullOrEmpty(json))
                {
                    ProcessedData = JsonSerializer.Deserialize<ExtractedData>(json);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deserializing processed data");
            }
        }
    }
}