using System.Text.RegularExpressions;
using Kotminer.Scrapper;

// var git = new GithubScrapper("jejikeh", "token", "t");
// await git.GetTestInfo();

var ch = new DvchScrapper();
await ch.GetData("b");
await ch.GetData("po");
await ch.GetData("re");

// var regex = new Regex("[a-zA-Z<>\\\"\\/\\.\\#\\=\\-\\(\\)&]|\\d{6,}");
// var inputText = File.ReadAllText("../../../output.txt");
// var outputText = regex.Replace(inputText, "");
// File.WriteAllText("../../../o.txt", outputText);