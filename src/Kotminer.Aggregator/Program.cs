using System.Text.RegularExpressions;
using Kotminer.Scrapper.Dvch;

var boards = new string[]
{
    "au", "bi", "biz", "bo", "c", "cc", "em",
    "fa", "fiz", "fl", "ftb", "hi", "me", "mg",
    "mlp", "mo", "mov", "mu", "ne", "psy", "re",
    "sci", "sf", "sn", "sp", "spc", "tv", "un", "w",
    "wh", "wm", "wp", "zog", "de", "di", "diy", "izd",
    "mus", "pa", "p", "wrk", "po", "news", "int", "hry",
    "ai", "gd", "hw", "mobi", "pr", "ra", "s", "t",
    "bg", "cg", "gacha", "gsg", "ruvn", "tes", "v",
    "vg", "wr", "fg", "fur", "gg", "ga", "hc", "e", 
    "fet", "sex", "fag", "a", "fd", "ja", "ma", "vn",
    "d", "b", "soc", "rf" 
};

await Parallel.ForEachAsync(boards, async (board, _) =>
{
    var scrapper = new DvchScrapper(board, new Regex("[a-zA-Z<>\\\"\\/\\;\\.\\#\\=\\-\\(\\)&]|\\d{6,}"));
    await scrapper.Scrap();
    await scrapper.Save();
});