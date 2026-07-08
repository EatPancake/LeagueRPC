using System.Net.Http.Json;
using System.Diagnostics;

namespace LeagueRPC;

public class LiveClientService
{
  private readonly HttpClient httpClient;
  private const string BaseUrl = "https://127.0.0.1:2999/liveclientdata";

  public LiveClientService()
  {
      var handler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
      };
      httpClient = new HttpClient(handler)
      {
        Timeout = TimeSpan.FromSeconds(2)
      };
  }

  public async Task<AllGameData?> GetAllGameDataAsync()
  {
    try
    {
      var output = await httpClient.GetFromJsonAsync<AllGameData>($"{BaseUrl}/allgamedata"); 
      Console.WriteLine(output);
      return output;
    }
    catch (HttpRequestException)
    {
     return null;
    }
    catch (TaskCanceledException)
    {
     return null;
    }
    catch (System.Text.Json.JsonException ex)
    {
      Debug.WriteLine($"JSON Error: {ex.Message}");
      return null;;
    }
  }

  public async Task<ActivePlayer?> GetActivePlayerAsync()
  {
    try
    {
     return await httpClient.GetFromJsonAsync<ActivePlayer>($"{BaseUrl}/activeplayer"); 
    }
    catch (HttpRequestException)
    {
     return null;
    }
    catch (TaskCanceledException)
    {
     return null;
    }
  }
  public async Task<PlayerScores?> GetPlayerScoresAsync(string riotId)
  {
    try
    {
      var encoded = Uri.EscapeDataString(riotId);
     return await httpClient.GetFromJsonAsync<PlayerScores>($"{BaseUrl}/playerscores?riotId={encoded}"); 
    }
    catch (HttpRequestException)
    {
     return null;
    }
    catch (TaskCanceledException)
    {
     return null;
    }
    catch (System.Text.Json.JsonException ex)
    {
      Console.WriteLine($"JSON Error: {ex.Message}");
      return null;
    }
  }
}


public class AllGameData
{
  public ActivePlayer? ActivePlayer { get; set; }
  public IEnumerable<Player>? AllPlayers { get; set; }
  public GameData? GameData { get; set; }
}

public class GameData
{
  public string? GameMode { get; set; }
  public string? MapName { get; set; }
  public float GameTime { get; set; }
}
public class ActivePlayer
{
  public string? RiotId { get; set;}
  public int Level { get; set; }
}

public class PlayerScores
{
  public string? RiotId { get; set; }
  public int Assists { get; set; }
  public int Kills { get; set; }
  public int Deaths { get; set; }
  
}

public class Player
{
  public string? ChampionName { get; set; }
  public string? SummonerName { get; set; }
  public string? Team { get; set; }
  public string? Position { get; set; }
  public int SkinID { get; set; }
}