using DiscordRPC;
namespace LeagueRPC;

public class DiscordPresence
{
    private readonly DiscordRpcClient client;

    public DiscordPresence(string applicationId)
    {
        client = new DiscordRpcClient(applicationId);
        client.Initialize();
        Console.WriteLine("Discord RPC initialized.");
    }

    public void UpdatePresence(string details, string state, string largeImageKey, string largeImageText, string smallImageKey, string smallImageText,DateTime? startTime = null)
    {
        var presence = new RichPresence()
        {
            Details = details,
            State = state,
            Timestamps = new Timestamps()
            {
                Start = startTime
            },
            Assets = new Assets()
            {
                LargeImageKey = largeImageKey,
                LargeImageText = largeImageText,
                SmallImageKey = smallImageKey,
                SmallImageText = smallImageText
            }
        };

        client.SetPresence(presence);
    }

    public void ClearPresence()
    {
        client.ClearPresence();
    }

    public void Dispose()
    {
        client.Dispose();
    }
}