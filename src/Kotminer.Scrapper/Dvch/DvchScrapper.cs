using System.Text;
using System.Text.RegularExpressions;
using Kotminer.Scrapper.Dvch.Models;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Thread = Kotminer.Scrapper.Dvch.Models.Thread;

namespace Kotminer.Scrapper.Dvch;

public class DvchScrapper : IScrapper
{
    private readonly HttpClient _client = new HttpClient();
    private readonly string _board;
    private StringBuilder _result;

    private readonly Regex _regex;

    public DvchScrapper(string board, Regex regex)
    {
        _board = board;
        _regex = regex;
        _result = new StringBuilder();
        _client = new HttpClient()
        {
            BaseAddress = new Uri("https://2ch.hk/")
        };
    }
    
    public async Task Scrap(ILogger logger)
    {
        var data = await _client.GetAsync($"{_board}/threads.json");
        var boardData = await JsonSerializer.DeserializeAsync<BoardData>(await data.Content.ReadAsStreamAsync());
        var threadNum = 0;
        
        logger.LogInformation($"Start Scrapping the {_board} board!");

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
            
            logger.LogInformation($"Start Scrapping the {thread.Num} thread with {threadData.Posts.Length} number of posts!");

            var prompts = new Dictionary<int, string>();
            foreach (var post in threadData.Posts)
            {
                if(post is null)
                    continue;
                
                if(!prompts.ContainsKey(post.Num))
                    prompts.Add(post.Num, _regex.Replace(post.Comment, ""));
                
                foreach (var prompt in prompts.Where(prompt => post.Comment.Contains(prompt.Key.ToString())))
                    _result.Append(@"{ ""prompt"": """ + prompt.Value + @""", ""completion"":  """ + prompts[post.Num] + @""" }" + "\n");

                logger.LogInformation($"\t{threadNum}: Parsed the Post {post.Num} in the {thread.Num} thread, board {_board}, overall {boardData.Threads.Length}: \n\t", Encoding.UTF8);
            }

            threadNum++;
        }
    }

    public async Task Save()
    {
        if(!Directory.Exists("../../../dvch"))
            Directory.CreateDirectory("../../../dvch");
        
        await File.AppendAllTextAsync($"../../../dvch/{_board}.output.jsonl", _result.ToString()
            .Replace("(OP)", "")
            .Replace("&gt;", "")
            .Replace("&quot;", ""));
    }
}