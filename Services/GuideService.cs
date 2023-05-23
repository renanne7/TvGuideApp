using TvGuideApp.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace TvGuideApp.Services;

public static class GuideService
{
    static HttpClient client = new HttpClient();
    static string SingleSearchPath = "https://api.tvmaze.com/singlesearch/shows?q=";
    static string MultiSearchPath ="https://api.tvmaze.com/search/shows?q=";
    static string ShowsPlayingToday = "https://api.tvmaze.com/schedule?country=GB&date=";

    static async Task<Guide> GetJsonAsync(string path)
    {
        string ShowsJson = await client.GetStringAsync(path);   
        var tvShowData = JsonConvert.DeserializeObject<Guide>(ShowsJson);
        return tvShowData;
    }
    static async Task<IEnumerable<Guide>> GetJsonMultiAsync(string path)
    {
        string ShowsJson = await client.GetStringAsync(path);   
        var tvShowData = JsonConvert.DeserializeObject<IEnumerable<Guide>>(ShowsJson);
        return tvShowData;
    }
    static string GetTodayDate()
    {
        DateTime utcDate = DateTime.UtcNow;
        String cultureNames = "en-GB";
        var culture = new CultureInfo(cultureNames);
        var date = DateTime.Parse(utcDate.ToString(culture)).ToString("yyyy-MM-dd"); 
        return date;
    }
    public static async Task<Guide> GetShow(string name)
    {
        string url = SingleSearchPath + name;
        var tvShowData = await GetJsonAsync(url);
        return tvShowData;
    }
    public static async Task<IEnumerable<Guide>> GetShows(string name)
    {
        string url = MultiSearchPath + name;
        var tvShowData = await GetJsonMultiAsync(url);
        return tvShowData;
    }
    public static async Task<IEnumerable<Guide>> PlayingNow()
    {
        var date = GetTodayDate();
        string url = ShowsPlayingToday + date;
        var tvShowData = await GetJsonMultiAsync(url);
        return tvShowData;
    }
    public static async Task<IEnumerable<string>> ChannelList()
    {
        var tvShowData = await PlayingNow();
        
        List<string> AvailableWebChannels = new List<string>();
        foreach (var eachshow in tvShowData)
        {
            if(eachshow.Show != null)
            {
                if(eachshow.Show.WebChannel != null)
                {
                    if(eachshow.Show.WebChannel.Name != null)
                    {
                        AvailableWebChannels.Add(eachshow.Show.WebChannel.Name);
                    }
                }
            }
        }
        IEnumerable<string> DistinctWebChannels = AvailableWebChannels.Select(x => x).Distinct();
        return DistinctWebChannels;
    }
    public static async Task<IEnumerable<Guide>> ShowsInChannel(string channelname)
    {
        var tvShowData = await PlayingNow();

        return tvShowData.Where(p => p.Show?.WebChannel?.Name == channelname);
    }
}