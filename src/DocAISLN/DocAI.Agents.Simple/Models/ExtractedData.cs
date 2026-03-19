namespace DocAI.Console.Models;

public class ExtractedData
{
    public string FileName { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
    public List<Person> Persons { get; set; } = [];
    public List<EmailAddress> Emails { get; set; } = [];
    public List<TableData> Tables { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = new();
}
