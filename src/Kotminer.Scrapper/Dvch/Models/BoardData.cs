using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Dvch.Models;

public class BoardData
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("threads")] public ThreadMin?[] Threads { get; set; }
}