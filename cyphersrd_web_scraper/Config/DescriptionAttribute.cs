﻿namespace cyphersrd_web_scraper;

public class DescriptionAttribute : Attribute
{
    public string Description;

    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}