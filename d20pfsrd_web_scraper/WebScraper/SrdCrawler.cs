﻿using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace cyphersrd_web_scraper;

public class SrdCrawler
{
    public SrdCrawler()
    {
        Web = new HtmlWeb();
    }

    private HtmlWeb Web { get; }

    public void CrawlSitemap()
    {
        string url = GameSystem.GameSystemToLink[Program.System] + "/sitemap.xml.gz";

        List<string> subSitemapLinks = CrawlSubSitemaps(url);
        List<string> contentLinks = new List<string>();

        foreach (string subSitemapLink in subSitemapLinks)
        {
            Console.WriteLine($"Crawling sub sitemap: {subSitemapLink}");
            contentLinks.AddRange(CrawlSubSitemaps(subSitemapLink));
        }

        File.WriteAllLines(PathHelper.Combine(Program.RunLocation, Program.ContentLinksFileName), contentLinks.ToArray());

        Console.WriteLine("\nFinished crawling site maps. Starting crawling of pages...\n");

        List<string> failedLinks = new List<string>();
        int i = 0;
        foreach (string contentLink in contentLinks)
        {
            try
            {
                Console.WriteLine($"Crawling {i} of {contentLinks.Count}");
                Page page = CrawlPage(contentLink);
                page.Save();
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                failedLinks.Add(contentLink);
            }

            i++;
        }

        Console.WriteLine($"\nFinished crawling pages. Failed links count: {failedLinks.Count}");

        File.WriteAllLines(PathHelper.Combine(Program.RunLocation, Program.FailedLinksFileName), failedLinks.ToArray());
    }

    private List<string> CrawlSubSitemaps(string url)
    {
        WebClient wc = new WebClient();
        wc.Encoding = Encoding.UTF8;
        string sitemapString = wc.DownloadString(url);

        // Console.WriteLine(sitemapString);

        string pattern = @"<loc.*>(.*?)<\/loc>";
        Regex regex = new Regex(pattern);
        MatchCollection matchCollection = regex.Matches(sitemapString);

        List<string> links = new List<string>();

        foreach (Match match in matchCollection)
        {
            string a = match.Value.Substring(5);
            a = a.Remove(a.Length - 6);

            bool filter = false;
            foreach (string s in Program.DomainBlackList)
            {
                if (a.Contains(s))
                {
                    filter = true;
                }
            }

            if (!filter)
            {
                links.Add(a);
            }
        }

        return links;
    }

    private Page CrawlPage(string url)
    {
        Web.OverrideEncoding = Encoding.UTF8;
        HtmlDocument document = Web.Load(url);
        HtmlNodeCollection htmlNodeCollection = document.DocumentNode.SelectNodes("//div[@class='article-content']");
        string title = document.DocumentNode.SelectSingleNode("html/head/title").InnerText;

        if (htmlNodeCollection.Count > 0)
        {
            return new Page(title, url, htmlNodeCollection[0].InnerHtml, DateTime.Now);
        }

        return new Page(title, url, "", DateTime.Now);
    }
}