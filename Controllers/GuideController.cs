using TvGuideApp.Models;
using TvGuideApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace TvGuideApp.Controllers;

[ApiController]
[Route("[controller]")]
public class GuideController : ControllerBase
{
    [HttpGet("singlesearch/{name}")]
    public async Task<ActionResult<Guide>> GetShow(string name) 
    {
        Guide guide = await GuideService.GetShow(name);

        if (guide == null) 
            return NotFound();

        return Ok(guide);    
    }

    [HttpGet("multisearch/{name}")]
    public async Task<ActionResult<IEnumerable<Guide>>> GetShows(string name) 
    {
        IEnumerable<Guide> guide = await GuideService.GetShows(name);

        if (guide == null) 
            return NotFound();

        return Ok(guide);    
    }
    
    [HttpGet("playingnow")]
    public async Task<ActionResult<IEnumerable<Guide>>> PlayingNow() 
    {
        IEnumerable<Guide> guide = await GuideService.PlayingNow();

        if (guide == null) 
            return NotFound();

        return Ok(guide);    
    }

    [HttpGet("channels")]
    public async Task<ActionResult<IEnumerable<Guide>>> ChannelList() 
    {
        IEnumerable<string> guide = await GuideService.ChannelList();

        if (guide == null) 
            return NotFound();

        return Ok(guide);    
    }

    [HttpGet("channels/{Channelname}")]
    public async Task<ActionResult<IEnumerable<Guide>>> ShowsInChannel(string Channelname) 
    {
        IEnumerable<Guide> guide = await GuideService.ShowsInChannel(Channelname);

        if (guide == null) 
            return NotFound();

        return Ok(guide);    
    }
}