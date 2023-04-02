namespace Kotminer.Scrapper;

public interface IScrapper
{
    public Task Scrap();
    public Task Save();
}