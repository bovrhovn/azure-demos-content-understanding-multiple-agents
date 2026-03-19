namespace DocAI.Models;

public class TableData
{
    public string Title { get; set; } = string.Empty;
    public List<string> Headers { get; set; } = [];
    public List<List<string>> Rows { get; set; } = [];
    public int RowCount => Rows.Count;
    public int ColumnCount => Headers.Count;
}
