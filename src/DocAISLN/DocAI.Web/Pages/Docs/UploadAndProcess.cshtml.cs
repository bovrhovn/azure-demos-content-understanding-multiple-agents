using System.Text.Json;
using DocAI.Models;
using DocAI.Services.Data;
using DocAI.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocAI.Web.Pages.Docs;

public class UploadAndProcessPageModel(
    ILogger<UploadAndProcessPageModel> logger,
    IDocumentProcessingService documentProcessingService,
    ProcessDataService dataService) : PageModel
{
    [BindProperty]
    public IFormFile? FormFile { get; set; }
    
    public List<FileInfo> UploadedFiles { get; set; } = [];
    public ExtractedData? ProcessedData { get; set; }
    public ValidationResult? ValidationResult { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsProcessing { get; set; }

    public void OnGet()
    {
        logger.LogInformation("Accessed Upload and Process page at {DateLoaded}", DateTime.UtcNow);
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

        return RedirectToPage("/Docs/UploadAndProcess");
    }

    public async Task<IActionResult> OnPostProcessAsync(string fileName)
    {
        logger.LogInformation("Process requested for file: {FileName}", fileName);

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
            
            // Process the document using the agent pipeline
            logger.LogInformation("Starting document processing for: {FileName}", fileName);
            var (data, validation) = await documentProcessingService.ProcessPdfAsync(filePath);
            
            // Store results in TempData
            var extractedData = JsonSerializer.Serialize(data);
            var validationResult = JsonSerializer.Serialize(validation);
            dataService.SaveProcessedData(extractedData, validationResult);
            
            TempData["ProcessedFileName"] = fileName;
            TempData["SuccessMessage"] = $"Document '{fileName}' processed successfully!";
            
            logger.LogInformation("Document processed successfully: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing file: {FileName}", fileName);
            TempData["ErrorMessage"] = $"Error processing '{fileName}': {ex.Message}";
        }

        return RedirectToPage("/Docs/UploadAndProcess");
    }

    public IActionResult OnPostClearResults()
    {
        dataService.Clear();
        TempData.Remove("ProcessedFileName");
        TempData["SuccessMessage"] = "Results cleared";
        return RedirectToPage("/Docs/UploadAndProcess");
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

        var (extractedData, validationResult) = dataService.GetFromMemory();
        if (validationResult != null)
        {
            ValidationResult = validationResult;
        }
        if (extractedData != null)
        {
            ProcessedData = extractedData;
        }
    }
}