using System.Text;
using Octokit;

namespace Kotminer.Scrapper;

public class GithubScrapper
{
    private string _username;
    private GitHubClient _client;
    private HttpClient _httpClient;

    public GithubScrapper(string username, string clientSecret, string appName)
    {
        _username = username;
        _client = new GitHubClient(new ProductHeaderValue(appName));
        _client.Credentials = new Credentials(clientSecret);
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://raw.githubusercontent.com/")
        };
    }

    public async Task GetTestInfo()
    {
        var userData = await _client.Repository.GetAllForUser(_username);
        foreach (var r in userData.Where(x => !x.Fork))
        {
            var content = await _client.Repository.Content.GetAllContents(r.Id);
            Console.WriteLine("Repo "+ r.Name);
            await PrintContent(content, r.Id);
        }
    }

    private async Task PrintContent(IReadOnlyList<RepositoryContent> content, long idRepository)
    {
        foreach (var c in content)
        {
            if (c.Type == "File" && (c.Name.EndsWith(".cs")))
            {
                var fileContent = await _httpClient.GetAsync(c.DownloadUrl.Replace("https://raw.githubusercontent.com/", ""));
                Console.WriteLine("\t File: " + c.Path);
                await File.AppendAllTextAsync("../../../output.txt", await fileContent.Content.ReadAsStringAsync());
            } 
            else if (c.Type == "Dir")
                await PrintContent(await _client.Repository.Content.GetAllContents(idRepository, c.Path), idRepository);
        }
    }
}