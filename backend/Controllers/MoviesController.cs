using CineStream.Models;
using CineStream.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineStream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly TmdbService _tmdbService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(TmdbService tmdbService, ILogger<MoviesController> logger)
    {
        _tmdbService = tmdbService;
        _logger = logger;
    }

    /// <summary>
    /// Get popular movies from TMDb
    /// </summary>
    [HttpGet("popular")]
    public async Task<ActionResult<TmdbResponse<Movie>>> GetPopular([FromQuery] int page = 1)
    {
        try
        {
            page = Math.Clamp(page, 1, 500);
            var result = await _tmdbService.GetPopularMoviesAsync(page);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching popular movies");
            return StatusCode(500, new { detail = "Error fetching popular movies" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching popular movies");
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Get top rated movies from TMDb
    /// </summary>
    [HttpGet("top-rated")]
    public async Task<ActionResult<TmdbResponse<Movie>>> GetTopRated([FromQuery] int page = 1)
    {
        try
        {
            page = Math.Clamp(page, 1, 500);
            var result = await _tmdbService.GetTopRatedMoviesAsync(page);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching top rated movies");
            return StatusCode(500, new { detail = "Error fetching top rated movies" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching top rated movies");
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Search movies from TMDb
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<TmdbResponse<Movie>>> Search([FromQuery] string query, [FromQuery] int page = 1)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 1)
            {
                return BadRequest(new { detail = "Query parameter is required and must be at least 1 character" });
            }

            page = Math.Clamp(page, 1, 500);
            var result = await _tmdbService.SearchMoviesAsync(query, page);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when searching movies");
            return StatusCode(500, new { detail = "Error searching movies" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when searching movies");
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }

    /// <summary>
    /// Get movie details by ID
    /// </summary>
    [HttpGet("{movieId:int}")]
    public async Task<ActionResult<MovieDetails>> GetDetails(int movieId)
    {
        try
        {
            var result = await _tmdbService.GetMovieDetailsAsync(movieId);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound(new { detail = $"Movie with ID {movieId} not found" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "TMDb API error when fetching movie {MovieId}", movieId);
            return StatusCode(500, new { detail = "Error fetching movie details" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when fetching movie {MovieId}", movieId);
            return StatusCode(500, new { detail = "Internal server error" });
        }
    }
}
