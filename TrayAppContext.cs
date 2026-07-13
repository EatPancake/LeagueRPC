using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace LeagueRPC;
public class TrayAppContext : ApplicationContext
{
  private readonly NotifyIcon trayIcon;
  private readonly DiscordPresence discordPresence = new("1402418696126992445");
  private readonly LiveClientService liveClientService = new();
  private readonly System.Windows.Forms.Timer _pollTimer;



  private async Task PollAsync()
  {
    var activePlayer = await liveClientService.GetActivePlayerAsync();
    if (activePlayer?.RiotId == null)
    {
      discordPresence.ClearPresence();
      return;
    }

    var scoresTask = liveClientService.GetPlayerScoresAsync(activePlayer.RiotId);
    var allDataTask = liveClientService.GetAllGameDataAsync();
    await Task.WhenAll(scoresTask, allDataTask);

    var scores = scoresTask.Result;
    var data = allDataTask.Result;

    var me = data?.AllPlayers?.FirstOrDefault(p => p.SummonerName == activePlayer.RiotId)?.SummonerName;
    var pos = data?.AllPlayers?.FirstOrDefault(p => p.SummonerName == me)?.Position ?? "unknown";
    var champion = data?.AllPlayers?.FirstOrDefault(p => p.SummonerName == me)?.ChampionName ?? "unknown";
    var skin = data?.AllPlayers?.FirstOrDefault(p => p.SummonerName == me)?.SkinID.ToString() ?? "unknown";
    var kda = scores != null ? $"{scores.Kills}/{scores.Deaths}/{scores.Assists}" : "0/0/0";

    var mapName = GetMapName(data?.GameData?.MapName);

    var gameTime = data?.GameData?.GameTime ?? 0;
    var gameStartTime = DateTime.UtcNow.AddSeconds(-gameTime);

    discordPresence.UpdatePresence(
      details: $"In game {mapName}",
      state: $"{champion}, Level {activePlayer.Level}, KDA: {kda}",
      largeImageKey: $"https://raw.communitydragon.org/latest/game/assets/characters/{champion.ToLower()}/hud/{champion.ToLower()}_circle{GetSkinNumString(skin, champion)}.png",
      largeImageText: $"Riot ID: {activePlayer.RiotId ?? "unknown"}",
      smallImageKey: $"{MapPositionToImageKey(pos)}",
      smallImageText: $"{pos.ToUpper()}",
      startTime: gameStartTime
    );
  }
  public TrayAppContext()
  {
      var menu = new ContextMenuStrip();
      menu.Items.Add("League RPC", null, null);
      menu.Items.Add("Exit", null, OnExit);

      trayIcon = new NotifyIcon
      {
          Icon = LoadTrayIcon(),
          ContextMenuStrip = menu,
          Visible = true,
          Text = "League RPC"
      };

      _pollTimer = new System.Windows.Forms.Timer { Interval = 5000 };
      _pollTimer.Tick += async (sender, e) => await PollAsync();
      _pollTimer.Start();
  }

  private static Icon LoadTrayIcon()
  {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = assembly.GetManifestResourceNames()
          .FirstOrDefault(n => n.EndsWith("icon.ico", StringComparison.OrdinalIgnoreCase));

      if (resourceName == null)
          return SystemIcons.Application;

      using var stream = assembly.GetManifestResourceStream(resourceName);
      return stream != null ? new Icon(stream) : SystemIcons.Application;
  }
  void OnExit(object? sender, EventArgs e)
  {
      discordPresence.ClearPresence();
      discordPresence.Dispose();
      trayIcon.Visible = false;
      Application.Exit();
  }

  private static string MapPositionToImageKey(string position)
  {
      var pos = position.ToLower() switch
      {
          "top" => "top",
          "jungle" => "jungle",
          "middle" => "mid",
          "bottom" => "bot",
          "utility" => "support",
          _ => "unknown"
      };
      if (pos == "unknown") return "";
      return $"https://raw.communitydragon.org/latest/game/assets/ux/lol/rolequest_icon{pos}32.png";
  }

  private static readonly Dictionary<string, string> MapNames = new()
  {
      { "Map1", "Summoner's Rift (Old)" },
      { "Map8", "The Crystal Scar" },
      { "Map10", "Twisted Treeline" },
      { "Map11", "Summoner's Rift" },
      { "Map12", "Howling Abyss" },
      { "Map18", "Cosmic Ruins" },
      { "Map19", "Nexus Blitz" },
      { "Map21", "Convergence" },
      { "Map22", "Teamfight Tactics" },
      { "Map30", "Arena" }
  };

  private static string GetMapName(string? mapName)
  {
      if (mapName == null) return "Unknown Map";
      return MapNames.TryGetValue(mapName, out var map) ? map : mapName;
  }

  private static string GetSkinNumString(string? skinNum, string? championName)
  {
    if (skinNum == null || championName == null) return  "";
    if ((championName.ToLower() == "hecarim" || championName.ToLower() == "ahri" || championName.ToLower() == "zyra" || championName.ToLower() == "lulu") && skinNum == "0") return "_0";
    if (skinNum == "0") return  "";

    return "_" + skinNum;
  }
  
}