using CineStream.Models;
using CineStream.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineStream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TvShowsController : ControllerBase
{
    private readonly TmdbService _tmdbService;
    private readonly ILogger<TvShowsController> _logger;

    public TvShowsController(TmdbService tmdbService, ILogger<TvShowsController> logger)
    {
        _tmdbService = tmdbService;
        _logger = logger;
    }

    /// <summary>
    /// Get popular TV shows from TMDb
    /// </summary>
    [HttpGet("popular")]
    public async Task<ActionResult<TmdbResponse<TvShow>>> GetPopular([FromQuery] int page = 1)
    {
        try
        {
            page = Math.Clamp(page, 1, 500);
            var result = await _tmdbService.GetPopularTvShowsAsync(page);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching popular TV shows");
            return StatusCode(500, new { detail = "Error fetching popular TV shows" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching popular TV shows");
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Search TV shows from TMDb
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<TmdbResponse<TvShow>>> Search([FromQuery] string query, [FromQuery] int page = 1)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 1)
            {
                return BadRequest(new { detail = "Query parameter is required and must be at least 1 character" });
            }

            page = Math.Clamp(page, 1, 500);
            var result = await _tmdbService.SearchTvShowsAsync(query, page);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when searching TV shows");
            return StatusCode(500, new { detail = "Error searching TV shows" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when searching TV shows");
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Get TV show details by ID
    /// </summary>
    [HttpGet("{tvId:int}")]
    public async Task<ActionResult<TvShowDetails>> GetDetails(int tvId)
    {
        try
        {
            var result = await _tmdbService.GetTvShowDetailsAsync(tvId);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound(new { detail = $"TV show with ID {tvId} not found" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching TV show {TvId}", tvId);
            return StatusCode(500, new { detail = "Error fetching TV show details" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching TV show {TvId}", tvId);
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Get season details for a TV show
    /// </summary>
    [HttpGet("{tvId:int}/season/{seasonNumber:int}")]
    public async Task<ActionResult<SeasonDetails>> GetSeason(int tvId, int seasonNumber)
    {
        try
        {
            var result = await _tmdbService.GetTvSeasonDetailsAsync(tvId, seasonNumber);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound(new { detail = $"Season {seasonNumber} of TV show {tvId} not found" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching season {SeasonNumber} of TV show {TvId}", seasonNumber, tvId);
            return StatusCode(500, new { detail = "Error fetching TV season details" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching season {SeasonNumber} of TV show {TvId}", seasonNumber, tvId);
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }
}
