using DocAI.Models;

namespace DocAI.Services.Data;

/// <summary>
/// Service for processing documents using multi-agent AI pipeline
/// </summary>
public interface IDocumentProcessingService
{
    /// <summary>
    /// Process a PDF document using the multi-agent pipeline
    /// </summary>
    /// <param name="filePath">Full path to the PDF file</param>
    /// <returns>Tuple containing extracted data and validation results</returns>
    Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(string filePath);
    
    /// <summary>
    /// Process a PDF document from a stream using the multi-agent pipeline
    /// </summary>
    /// <param name="fileStream">Stream containing the PDF data</param>
    /// <param name="fileName">Name of the file for identification</param>
    /// <returns>Tuple containing extracted data and validation results</returns>
    Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(Stream fileStream, string fileName);
}
