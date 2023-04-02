using System.Text;
using System.Text.RegularExpressions;
using Kotminer.Scrapper.Dvch.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Thread = Kotminer.Scrapper.Dvch.Models.Thread;

namespace Kotminer.Scrapper.Dvch;

public class DvchScrapper : IScrapper
{
    private readonly HttpClient _client = new HttpClient();
    private readonly string _board;
    private StringBuilder _result = new StringBuilder();
    private readonly Regex _regex;

    public DvchScrapper(string board, Regex regex)
    {
        _board = board;
        _regex = regex;
        _client = new HttpClient()
        {
            BaseAddress = new Uri("https://2ch.hk/")
        };
    }
    
    public async Task Scrap()
    {
        var data = await _client.GetAsync($"{_board}/threads.json");
        var boardData = await JsonSerializer.DeserializeAsync<BoardData>(await data.Content.ReadAsStreamAsync());
        
        Console.WriteLine($"Start Scrapping the {_board} board!");

        if (boardData?.Threads is null)
            return;
        
        foreach (var thread in boardData.Threads)
        {
            if (thread is null)
                continue;
        
            var threadDataResponse = await _client.GetAsync($"api/mobile/v2/after/{_board}/{thread.Num}/{thread.Num}");
            var threadData = await JsonSerializer.DeserializeAsync<Thread>(await threadDataResponse.Content.ReadAsStreamAsync());

            if (threadData?.Posts is null) 
                continue;
            
            Console.WriteLine($"Start Scrapping the {thread.Num} thread with {threadData.Posts.Length} number of posts!");
                
            foreach (var post in threadData.Posts)
            {
                if(post is null)
                    continue;

                var filteredData = _regex.Replace(post.Comment, " ");
                _result.Append(filteredData.Trim()).AppendLine("\n");
                Console.WriteLine($"\t Parsed the Post {post.Num} with following content: \n\t{filteredData}");
            }
        }
    }

    public async Task Save()
    {
        if(!Directory.Exists("../../../dvch"))
            Directory.CreateDirectory("../../../dvch");
        
        await File.AppendAllTextAsync($"../../../dvch/{_board}.output.txt", _result.ToString());
    }
}