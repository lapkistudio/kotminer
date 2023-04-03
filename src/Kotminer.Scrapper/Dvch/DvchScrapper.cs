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
    private DataType _dataType;
    private readonly Regex _regex;

    public DvchScrapper(string board, Regex regex, DataType dataType)
    {
        _board = board;
        _regex = regex;
        _dataType = dataType;
        _result = new StringBuilder("\n");
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
            threadNum++;
            if (thread is null)
                continue;

            var threadDataResponse = await _client.GetAsync($"api/mobile/v2/after/{_board}/{thread.Num}/{thread.Num}");
            var threadData =
                await JsonSerializer.DeserializeAsync<Thread>(await threadDataResponse.Content.ReadAsStreamAsync());

            if (threadData?.Posts is null)
                continue;

            logger.LogInformation(
                $"{threadNum}: Start Scrapping the {thread.Num} thread with {threadData.Posts.Length} number of posts! in /{_board} -> {boardData.Threads.Length}");

            switch (_dataType)
            {
                case DataType.OpenAiFineTuneModel:
                {
                    CreateOpenAiFineTuneData(threadData);
                    break;
                }
                case DataType.Plain:
                {
                    CreatePlainData(threadData);
                    break;
                }
                case DataType.XTuringModel:
                    CreateXTuringModelData(threadData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void CreatePlainData(Thread threadData)
    {
        foreach (var post in threadData.Posts)
        {
            if (post is null)
                continue;

            _result.Append(_regex.Replace(post.Comment, "") + "\n");
        }
    }

    private void CreateOpenAiFineTuneData(Thread threadData)
    {
        var prompts = new Dictionary<int, string>();
        foreach (var post in threadData.Posts)
        {
            if (post is null)
                continue;

            if (!prompts.ContainsKey(post.Num))
                prompts.Add(post.Num, _regex.Replace(post.Comment, ""));

            foreach (var prompt in prompts.Where(prompt => post.Comment.Contains(prompt.Key.ToString())))
                _result.Append(@"{ ""prompt"": """ + prompt.Value + @""", ""completion"":  """ +
                               prompts[post.Num] + @""" }" + "\n");
        }
    }
    
    private void CreateXTuringModelData(Thread threadData)
    {
        var prompts = new Dictionary<int, string>();
        var comma = "";
        foreach (var post in threadData.Posts)
        {
            if (post is null)
                continue;

            if (!prompts.ContainsKey(post.Num))
                prompts.Add(post.Num, _regex.Replace(post.Comment, ""));

            foreach (var prompt in prompts.Where(prompt => post.Comment.Contains(prompt.Key.ToString())))
            {
                _result.Append($"{comma}\t" +
                               "{\n\t\t" +
                               @"""instruction"": " + @"""Представь что ты отвечаешь анону на анонимном имиджборде""" + ",\n\t\t" +
                               @"""input"": """ + prompt.Value + @"""," + "\n\t\t" +
                               @"""output"": """ + prompts[post.Num] + @"""" +
                               "\n\t}");

                comma = ",\n";
            }
        }
    }

    public async Task Save()
    {
        if(!Directory.Exists("../../../dvch"))
            Directory.CreateDirectory("../../../dvch");

        switch (_dataType)
        {
            case DataType.Plain:
                await File.AppendAllTextAsync($"../../../dvch/{_board}.output.txt", _result.ToString()
                    .Replace("(OP)", "")
                    .Replace("&gt;", "")
                    .Replace("&quot;", ""));
                break;
            case DataType.OpenAiFineTuneModel:
                await File.AppendAllTextAsync($"../../../dvch/{_board}.output.jsonl", _result.ToString()
                    .Replace("(OP)", "")
                    .Replace("&gt;", "")
                    .Replace("&quot;", ""));
                break;
            case DataType.XTuringModel:
                await File.AppendAllTextAsync($"../../../dvch/{_board}.output.jsonl", _result.ToString()
                    .Replace("(OP)", "")
                    .Replace("&gt;", "")
                    .Replace("&quot;", ""));
                break;
        }
    }
}