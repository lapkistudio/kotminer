using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Dvch.Models;

public class ThreadMin
{
    [JsonPropertyName("num")] public int Num { get; set; }
}