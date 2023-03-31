using System.Text.Json;
using Kotminer.Scrapper.Models;
using Newtonsoft.Json;
using Wakaba2ChApiClient.Impl;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Thread = Kotminer.Scrapper.Models.Thread;
using Wakaba2ChApiClient = Wakaba2ChApiClient.Wakaba2ChApiClient;

namespace Kotminer.Scrapper;

public class DvchScrapper
{
    private readonly HttpClient _client = new HttpClient();

    public DvchScrapper()
    {
        _client = new HttpClient()
        {
            BaseAddress = new Uri("https://2ch.hk/")
        };
    }
    
    public async Task GetData(string t)
    {
        if(!File.Exists($"../../../{t}.output.txt"))
            File.Create($"../../../{t}.output.txt").Close();
        
        var data = await _client.GetAsync($"{t}/threads.json");
        var threadsFromBoard = await JsonSerializer.DeserializeAsync<BoardData>(await data.Content.ReadAsStreamAsync());
        if (threadsFromBoard is null)
            return;
        
        foreach (var thread in threadsFromBoard.Threads.Where(x => x != null))
        {
            var request = await _client.GetAsync($"api/mobile/v2/after/{t}/{thread.Num}/{thread.Num}");
            var threadData = await JsonSerializer.DeserializeAsync<Thread>(await request.Content.ReadAsStreamAsync());
            if (threadData is null)
                return;
            
            foreach (var post in threadData.Posts.Where(x => x != null))
            {
                await File.AppendAllTextAsync($"../../../{t}.output.txt", post.Comment + "\n");
                Console.WriteLine($"--- thread parsed {thread.Num}");
                Console.WriteLine(post.Comment);
            }
        }
    } 
}