using System.Globalization;
using DocAI.Console.Models;
using UglyToad.PdfPig;

namespace DocAI.Console.Helpers;

public class PdfService
{
    public ExtractedData ExtractText(string filePath)
    {
        var extractedData = new ExtractedData
        {
            FileName = Path.GetFileName(filePath)
        };

        using var document = PdfDocument.Open(filePath);
        var textBuilder = new System.Text.StringBuilder();
        
        foreach (var page in document.GetPages())
        {
            textBuilder.AppendLine(page.Text);
            textBuilder.AppendLine(); // Add spacing between pages
        }

        extractedData.RawText = textBuilder.ToString();
        extractedData.Metadata["PageCount"] = document.NumberOfPages.ToString();
        extractedData.Metadata["PdfVersion"] = document.Version.ToString(CultureInfo.InvariantCulture);

        return extractedData;
    }
}