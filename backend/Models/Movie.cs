namespace CineStream.Models;

public class TmdbResponse<T>
{
    public int Page { get; set; }
    public List<T> Results { get; set; } = new();
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Overview { get; set; } = "";
    public string PosterPath { get; set; } = "";
    public string BackdropPath { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public List<int> GenreIds { get; set; } = new();
    public bool Adult { get; set; }
    public string OriginalLanguage { get; set; } = "";
    public string OriginalTitle { get; set; } = "";
    public double Popularity { get; set; }
    public bool Video { get; set; }

    // Computed properties for full URLs
    public string PosterUrl { get; set; } = "";
    public string BackdropUrl { get; set; } = "";
}

public class TvShow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string PosterPath { get; set; } = "";
    public string BackdropPath { get; set; } = "";
    public string FirstAirDate { get; set; } = "";
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public List<int> GenreIds { get; set; } = new();
    public List<string> OriginCountry { get; set; } = new();
    public string OriginalLanguage { get; set; } = "";
    public string OriginalName { get; set; } = "";
    public double Popularity { get; set; }

    public string PosterUrl { get; set; } = "";
    public string BackdropUrl { get; set; } = "";
}

public class MovieDetails : Movie
{
    public int Runtime { get; set; }
    public string Tagline { get; set; } = "";
    public string Status { get; set; } = "";
    public long Budget { get; set; }
    public long Revenue { get; set; }
    public List<Genre> Genres { get; set; } = new();
    public Credits Credits { get; set; } = new();
    public Videos Videos { get; set; } = new();
    public string VidsrcEmbedUrl { get; set; } = "";
}

public class TvShowDetails : TvShow
{
    public int NumberOfSeasons { get; set; }
    public int NumberOfEpisodes { get; set; }
    public string Tagline { get; set; } = "";
    public string Status { get; set; } = "";
    public List<Genre> Genres { get; set; } = new();
    public List<Season> Seasons { get; set; } = new();
    public Credits Credits { get; set; } = new();
    public Videos Videos { get; set; } = new();
    public string VidsrcEmbedUrl { get; set; } = "";
}

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Credits
{
    public List<CastMember> Cast { get; set; } = new();
    public List<CrewMember> Crew { get; set; } = new();
}

public class CastMember
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Character { get; set; } = "";
    public int Order { get; set; }
    public string ProfilePath { get; set; } = "";
}

public class CrewMember
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Job { get; set; } = "";
    public string Department { get; set; } = "";
}

public class Videos
{
    public List<VideoResult> Results { get; set; } = new();
}

public class VideoResult
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Site { get; set; } = "";
    public string Type { get; set; } = "";
}

public class Season
{
    public string AirDate { get; set; } = "";
    public int EpisodeCount { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string PosterPath { get; set; } = "";
    public int SeasonNumber { get; set; }
}

public class SeasonDetails
{
    public string AirDate { get; set; } = "";
    public List<Episode> Episodes { get; set; } = new();
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public int Id { get; set; }
    public string PosterPath { get; set; } = "";
    public int SeasonNumber { get; set; }
}

public class Episode
{
    public string AirDate { get; set; } = "";
    public int EpisodeNumber { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string StillPath { get; set; } = "";
    public int Runtime { get; set; }
    public double Vote_Average { get; set; }
    public string Vidsrc_Embed_Url { get; set; } = "";
}

public class HealthResponse
{
    public string Status { get; set; } = "healthy";
    public string Message { get; set; } = "API is running";
}
