namespace DocAI.Models;

public class Person
{
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Organization { get; set; }
    public int Confidence { get; set; }
}
