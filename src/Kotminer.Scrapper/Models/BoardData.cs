using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Kotminer.Scrapper.Models;

public class BoardData
{
    [JsonPropertyName("board")] public string Board { get; set; }
    [JsonPropertyName("threads")] public ThreadMin?[] Threads { get; set; }
}