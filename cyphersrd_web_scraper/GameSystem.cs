namespace cyphersrd_web_scraper;

public static class GameSystem
{
    public const string CYPHER = "cypher";

    public static Dictionary<string, string> GameSystemToLink = new Dictionary<string, string>
    {
        {CYPHER, "https://callmepartario.github.io/og-csrd/"},
    };

    public static Dictionary<string, string> GameSystemToPrefix = new Dictionary<string, string>
    {
        {CYPHER, "cyphersrd"},
    };

    public static Dictionary<string, string> GameSystemToTitlePostfix = new Dictionary<string, string>
    {
        {CYPHER, " &#8211; cypherSRD"},
    };

    public static string ValidateSystem(string system)
    {
        system = system.ToLower();
        return system switch
        {
            CYPHER => CYPHER,
            _ => throw new ArgumentException("Not a valid Game System"),
        };
    }
}