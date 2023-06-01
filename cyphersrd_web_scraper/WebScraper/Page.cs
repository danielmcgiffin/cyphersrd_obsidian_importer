﻿using Newtonsoft.Json;

namespace cyphersrd_web_scraper;

public class Page
{
    public Page(string title, string url, string content, DateTime timeAccessed)
    {
        string titleFilter = GameSystem.GameSystemToTitlePostfix[Program.System];
        if (title.Contains(titleFilter))
        {
            title = title.Remove(title.Length - titleFilter.Length);
        }

        Title = title;
        URL = url;
        Content = content;
        TimeAccessed = timeAccessed;

        // File.WriteAllLines("test.html", new string[] { content });
    }

    public string Title { get; set; }
    public string URL { get; set; }

    [JsonIgnore] public string Content { get; set; }

    public DateTime TimeAccessed { get; set; }

    public void Save()
    {
        Uri uri = new Uri(URL);
        DirectoryInfo directoryInfo = Directory.CreateDirectory(PathHelper.Combine(Program.ScraperOutputLocation, uri.AbsolutePath));
        // Console.WriteLine(directoryInfo.FullName);
        File.WriteAllText(PathHelper.Combine(directoryInfo.FullName, "index.html"), Content);
        File.WriteAllText(PathHelper.Combine(directoryInfo.FullName, "meta.json"), JsonConvert.SerializeObject(this));
    }
}