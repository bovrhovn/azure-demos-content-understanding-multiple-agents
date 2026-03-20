using System.Text.Json;
using DocAI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DocAI.Web.Services;

public class ProcessDataService(IMemoryCache cache)
{
    private const string CacheKey = "ProcessedData";
    public bool SaveProcessedData(string processData, string validationResult)
    {
        //save raw data to cache 
        cache.Set(CacheKey, (processData, validationResult), TimeSpan.FromMinutes(30));
        return true;
    }

    public (ExtractedData? extractedData, ValidationResult? validationResult) GetFromMemory()
    {
        //get data from cache and return as ExtractedData and ValidationResult
        if (cache.TryGetValue(CacheKey, out (string processData, string validationResult) cachedData))
        {
            var extractedData = JsonSerializer.Deserialize<ExtractedData>(cachedData.processData);
            if (string.IsNullOrEmpty(cachedData.validationResult))
            {
                return (extractedData, null);
            }
            var validationResult = JsonSerializer.Deserialize<ValidationResult>(cachedData.validationResult);
            return (extractedData, validationResult);
        }
        return (null, null);
    }

    public void Remove()
    {
        cache.Remove(CacheKey);
    }
    
    public void Clear()
    {
        SaveProcessedData(string.Empty, string.Empty);
    }
}