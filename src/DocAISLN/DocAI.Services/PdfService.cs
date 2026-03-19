using System.Globalization;
using DocAI.Models;
using UglyToad.PdfPig;

namespace DocAI.Services;

public class PdfService
{
     public ExtractedData ExtractText(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file {filePath} was not found.");

        var extractedData = new ExtractedData
        {
            FileName = Path.GetFileName(filePath)
        };

        using var document = PdfDocument.Open(filePath);
        var textBuilder = new System.Text.StringBuilder();

        foreach (var page in document.GetPages())
        {
            textBuilder.AppendLine(page.Text);
            textBuilder.AppendLine();
        }

        extractedData.RawText = textBuilder.ToString();
        extractedData.Metadata["PageCount"] = document.NumberOfPages.ToString();
        extractedData.Metadata["PdfVersion"] = document.Version.ToString(CultureInfo.InvariantCulture);

        return extractedData;
    }

    public ExtractedData ExtractText(Stream fileStream)
    {
        var extractedData = new ExtractedData
        {
            FileName = Path.GetFileName(fileStream is FileStream fs ? fs.Name : "UploadedFile.pdf")
        };

        using var document = PdfDocument.Open(fileStream);
        var textBuilder = new System.Text.StringBuilder();

        foreach (var page in document.GetPages())
        {
            textBuilder.AppendLine(page.Text);
            textBuilder.AppendLine();
        }

        extractedData.RawText = textBuilder.ToString();
        extractedData.Metadata["PageCount"] = document.NumberOfPages.ToString();
        extractedData.Metadata["PdfVersion"] = document.Version.ToString(CultureInfo.InvariantCulture);

        return extractedData;
    }
}