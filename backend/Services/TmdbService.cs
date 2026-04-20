using System.Text.Json;
using System.Text.Json.Serialization;
using CineStream.Models;

namespace CineStream.Services;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TmdbService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _imageBaseUrl;
    private readonly string _backdropBaseUrl;

    public TmdbService(HttpClient httpClient, IConfiguration configuration, ILogger<TmdbService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _apiKey = configuration["Tmdb:ApiKey"] 
            ?? throw new InvalidOperationException("Tmdb:ApiKey is not configured");
        _baseUrl = configuration["Tmdb:BaseUrl"] ?? "https://api.themoviedb.org/3";
        _imageBaseUrl = configuration["Tmdb:ImageBaseUrl"] ?? "https://image.tmdb.org/t/p/w500";
        _backdropBaseUrl = configuration["Tmdb:BackdropBaseUrl"] ?? "https://image.tmdb.org/t/p/w1280";
    }

    private JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<TmdbResponse<Movie>> GetPopularMoviesAsync(int page = 1)
    {
        var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&page={page}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TmdbResponse<Movie>>(content, JsonOptions)
            ?? new TmdbResponse<Movie>();
        
        AddImageUrls(data.Results);
        return data;
    }

    public async Task<TmdbResponse<Movie>> GetTopRatedMoviesAsync(int page = 1)
    {
        var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&page={page}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TmdbResponse<Movie>>(content, JsonOptions)
            ?? new TmdbResponse<Movie>();
        
        AddImageUrls(data.Results);
        return data;
    }

    public async Task<TmdbResponse<Movie>> SearchMoviesAsync(string query, int page = 1)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={encodedQuery}&page={page}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TmdbResponse<Movie>>(content, JsonOptions)
            ?? new TmdbResponse<Movie>();
        
        AddImageUrls(data.Results);
        return data;
    }

    public async Task<MovieDetails> GetMovieDetailsAsync(int movieId)
    {
        var url = $"{_baseUrl}/movie/{movieId}?api_key={_apiKey}&append_to_response=credits,videos";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var movie = JsonSerializer.Deserialize<MovieDetails>(content, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize movie details");
        
        AddImageUrls(movie);
        movie.VidsrcEmbedUrl = $"https://vidsrc.me/embed/movie/{movieId}";
        
        return movie;
    }

    public async Task<TmdbResponse<TvShow>> GetPopularTvShowsAsync(int page = 1)
    {
        var url = $"{_baseUrl}/tv/popular?api_key={_apiKey}&page={page}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TmdbResponse<TvShow>>(content, JsonOptions)
            ?? new TmdbResponse<TvShow>();
        
        AddImageUrls(data.Results);
        return data;
    }

    public async Task<TmdbResponse<TvShow>> SearchTvShowsAsync(string query, int page = 1)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var url = $"{_baseUrl}/search/tv?api_key={_apiKey}&query={encodedQuery}&page={page}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TmdbResponse<TvShow>>(content, JsonOptions)
            ?? new TmdbResponse<TvShow>();
        
        AddImageUrls(data.Results);
        return data;
    }

    public async Task<TvShowDetails> GetTvShowDetailsAsync(int tvId)
    {
        var url = $"{_baseUrl}/tv/{tvId}?api_key={_apiKey}&append_to_response=credits,videos";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var show = JsonSerializer.Deserialize<TvShowDetails>(content, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize TV show details");
        
        AddImageUrls(show);
        show.VidsrcEmbedUrl = $"https://vidsrc.to/embed/tv/{tvId}";
        
        return show;
    }

    public async Task<SeasonDetails> GetTvSeasonDetailsAsync(int tvId, int seasonNumber)
    {
        var url = $"{_baseUrl}/tv/{tvId}/season/{seasonNumber}?api_key={_apiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var season = JsonSerializer.Deserialize<SeasonDetails>(content, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize season details");
        
        // Add VidSrc embed URLs for episodes
        foreach (var episode in season.Episodes)
        {
            episode.VidsrcEmbedUrl = $"https://vidsrc.me/embed/tv/{tvId}/{seasonNumber}/{episode.EpisodeNumber}";
        }
        
        return season;
    }

    // Helper methods
    private void AddImageUrls(List<Movie> movies)
    {
        foreach (var movie in movies)
        {
            AddImageUrls(movie);
        }
    }

    private void AddImageUrls(Movie movie)
    {
        if (!string.IsNullOrEmpty(movie.PosterPath))
            movie.PosterUrl = $"{_imageBaseUrl}{movie.PosterPath}";
        if (!string.IsNullOrEmpty(movie.BackdropPath))
            movie.BackdropUrl = $"{_backdropBaseUrl}{movie.BackdropPath}";
    }

    private void AddImageUrls(List<TvShow> shows)
    {
        foreach (var show in shows)
        {
            AddImageUrls(show);
        }
    }

    private void AddImageUrls(TvShow show)
    {
        if (!string.IsNullOrEmpty(show.PosterPath))
            show.PosterUrl = $"{_imageBaseUrl}{show.PosterPath}";
        if (!string.IsNullOrEmpty(show.BackdropPath))
            show.BackdropUrl = $"{_backdropBaseUrl}{show.BackdropPath}";
    }

    private void AddImageUrls(TvShowDetails show)
    {
        if (!string.IsNullOrEmpty(show.PosterPath))
            show.PosterUrl = $"{_imageBaseUrl}{show.PosterPath}";
        if (!string.IsNullOrEmpty(show.BackdropPath))
            show.BackdropUrl = $"{_backdropBaseUrl}{show.BackdropPath}";
    }
}
