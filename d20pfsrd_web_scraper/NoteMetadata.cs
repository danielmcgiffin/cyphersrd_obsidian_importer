﻿using System.Text;
using System.Text.Json;

namespace cyphersrd_web_scraper;

public class NoteMetadata
{
    public string FileName;
    public string LocalPathToFolder;
    public string LocalPathToHtml;
    public string LocalPathToJson;
    public string LocalPathToMarkdown;
    public string[] Tags;
    public string TimeAccessed;
    public string Title;
    public string Url;
    public string WebTitle;

    public NoteMetadata(string localPath, string mappedLocalPath)
    {
        LocalPathToFolder = PathHelper.TrimSlashes(mappedLocalPath);
        LocalPathToHtml = PathHelper.Combine(Program.HtmlFolder, localPath, "index.html");
        LocalPathToJson = PathHelper.Combine(Program.HtmlFolder, localPath, "meta.json");

        FileName = MdConverter.ConvertToMdTitle(mappedLocalPath);
        Title = MdConverter.GetDocumentHeadingFromMdTitle(FileName);
        Tags = FileName.Split('_')[..^1];

        LocalPathToMarkdown = PathHelper.Combine(Program.MarkdownFolder, mappedLocalPath, FileName + ".md");

        string meta = File.ReadAllText(Path.Combine(Program.RunLocation, LocalPathToJson));
        Page page = JsonSerializer.Deserialize<Page>(meta);

        WebTitle = page.Title;
        Url = page.URL;
        TimeAccessed = page.TimeAccessed.ToString();
    }

    public string ToMetadata()
    {
        return $@"---
title: {Title}
webTitle: {WebTitle}
fileName: {FileName}
localPathToFolder: {LocalPathToFolder}
localPathToHtml: {LocalPathToHtml}
localPathToJson: {LocalPathToJson}
localPathToMarkdown: {LocalPathToMarkdown}
url: {Url}
timeAccessed: {TimeAccessed}
tags: {TagsToMetadata()}
---
";
    }

    private string TagsToMetadata()
    {
        if (Tags.Length == 0)
        {
            return "[]";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("[ ");
        sb.Append(Tags[0]);

        for (int i = 1; i < Tags.Length; i++)
        {
            sb.Append(", ").Append(Tags[i]);
        }

        sb.Append(" ]");

        return sb.ToString();
    }
}