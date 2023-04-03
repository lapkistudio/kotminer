using Kotminer.Scrapper.Dvch;
using Microsoft.Extensions.Logging;

namespace Kotminer.Scrapper;

public interface IScrapper
{
    public Task Scrap(ILogger logger);
    public Task Save();
}