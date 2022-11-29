namespace Skeletrino.Bot.Settings;

public class Settings
{
    public string Token { get; set; }
    public IList<string> Prefixes { get; set; }

    public Settings(string token, IList<string> prefixes)
    {
        Token = token;
        Prefixes = prefixes;
    }
}