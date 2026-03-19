using DocAI.Models;

namespace DocAI.Services.Data;

/// <summary>
/// Service for processing documents using Azure Document Intelligence
/// </summary>
public interface IAzureDocIntelligenceService
{
    /// <summary>
    /// Analyze a PDF document using Azure Document Intelligence
    /// </summary>
    /// <param name="filePath">Full path to the PDF file</param>
    /// <returns>Extracted data containing pages, text, and tables</returns>
    Task<ExtractedData> AnalyzeDocumentAsync(string filePath);
    
    /// <summary>
    /// Analyze a PDF document from a stream using Azure Document Intelligence
    /// </summary>
    /// <param name="fileStream">Stream containing the PDF data</param>
    /// <param name="fileName">Name of the file for identification</param>
    /// <returns>Extracted data containing pages, text, and tables</returns>
    Task<ExtractedData> AnalyzeDocumentAsync(Stream fileStream, string fileName);
}
