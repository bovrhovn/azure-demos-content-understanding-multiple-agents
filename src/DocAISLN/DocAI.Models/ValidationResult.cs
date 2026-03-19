namespace DocAI.Models;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationItem> ValidItems { get; set; } = [];
    public List<ValidationItem> InvalidItems { get; set; } = [];
    public List<ValidationItem> UnconfirmedItems { get; set; } = [];
}

public class ValidationItem
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
