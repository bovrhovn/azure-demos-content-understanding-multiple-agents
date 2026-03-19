namespace DocAI.Services.General;

public class LocalStorageService
{
    public required string StoragePath { get; set; }
    //list file and return array of file list
    public string[] ListFiles()
    {
        var files = Directory.GetFiles(StoragePath);
        return (files.Length == 0 ? Array.Empty<string>() 
            : files.Select(Path.GetFileName).ToArray())!;
    }

    public void SaveFile(string fileName, byte[] content)
    {
        var filePath = Path.Combine(StoragePath, fileName);
        File.WriteAllBytes(filePath, content);
    }

    public byte[] GetFile(string fileName)
    {
        var filePath = Path.Combine(StoragePath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllBytes(filePath);
        }
        throw new FileNotFoundException($"File {fileName} not found in local storage.");
    }

    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(StoragePath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}