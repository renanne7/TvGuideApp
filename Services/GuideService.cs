using TvGuideApp.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;

namespace TvGuideApp.Services;

public interface IMyDependency
{
    Task<Guide> GetShow(string name);
    Task<IEnumerable<Guide>> GetShows(string name);
    Task<IEnumerable<Guide>> PlayingNow();
    Task<IEnumerable<string>> ChannelList();
    Task<IEnumerable<Guide>> ShowsInChannel(string channelname);
}
public class GuideService : IMyDependency
{
    private string SingleSearchPath = "singlesearch/shows?q=";
    private string MultiSearchPath ="search/shows?q=";
    private string ShowsPlayingToday = "schedule?country=GB&date=";
    private readonly IHttpClientFactory _httpClientFactory;
    public GuideService(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    private async Task<Guide> GetJsonAsync(string path)
    {

        var httpClient = _httpClientFactory.CreateClient("tvmaze");
        string ShowsJson = await httpClient.GetStringAsync(path);
        var tvShowData = JsonConvert.DeserializeObject<Guide>(ShowsJson);
        return tvShowData;
    }
    private async Task<IEnumerable<Guide>> GetJsonMultiAsync(string path)
    {
        var httpClient = _httpClientFactory.CreateClient("tvmaze");
        string ShowsJson = await httpClient.GetStringAsync(path); 
        var tvShowData = JsonConvert.DeserializeObject<IEnumerable<Guide>>(ShowsJson);
        return tvShowData;
    }
    private string GetTodayDate()
    {
        DateTime utcDate = DateTime.UtcNow;
        String cultureNames = "en-GB";
        var culture = new CultureInfo(cultureNames);
        var date = DateTime.Parse(utcDate.ToString(culture)).ToString("yyyy-MM-dd"); 
        return date;
    }
    public async Task<Guide> GetShow(string name)
    {
        string url = SingleSearchPath + name;
        var tvShowData = await GetJsonAsync(url);
        return tvShowData;
    }
    public async Task<IEnumerable<Guide>> GetShows(string name)
    {
        string url = MultiSearchPath + name;
        var tvShowData = await GetJsonMultiAsync(url);
        return tvShowData;
    }
    public async Task<IEnumerable<Guide>> PlayingNow()
    {
        var date = GetTodayDate();
        string url = ShowsPlayingToday + date;
        var tvShowData = await GetJsonMultiAsync(url);
        return tvShowData;
    }
    public async Task<IEnumerable<string>> ChannelList()
    {
        var tvShowData = await PlayingNow();
        
        List<string> AvailableWebChannels = new List<string>();
        foreach (var eachshow in tvShowData)
        {
            if(eachshow.Show is not null && eachshow.Show.WebChannel is not null && eachshow.Show.WebChannel.Name is not null)
            {
                AvailableWebChannels.Add(eachshow.Show.WebChannel.Name);
            }

        }
        IEnumerable<string> DistinctWebChannels = AvailableWebChannels.Select(x => x).Distinct();
        return DistinctWebChannels;
    }
    
    public async Task<IEnumerable<Guide>> ShowsInChannel(string channelname)
    {
        var tvShowData = await PlayingNow();

        return tvShowData.Where(p => p.Show?.WebChannel?.Name == channelname);
    }
}