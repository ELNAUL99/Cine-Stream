namespace CineStream.Models;

public class TmdbResponse<T>
{
    public int Page { get; set; }
    public List<T> Results { get; set; } = new();
    public int Total_Pages { get; set; }
    public int Total_Results { get; set; }
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Poster_Path { get; set; } = "";
    public string Backdrop_Path { get; set; } = "";
    public string Release_Date { get; set; } = "";
    public double Vote_Average { get; set; }
    public int Vote_Count { get; set; }
    public List<int> Genre_Ids { get; set; } = new();
    public bool Adult { get; set; }
    public string Original_Language { get; set; } = "";
    public string Original_Title { get; set; } = "";
    public double Popularity { get; set; }
    public bool Video { get; set; }

    // Computed properties for full URLs
    public string Poster_Url { get; set; } = "";
    public string Backdrop_Url { get; set; } = "";
}

public class TvShow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Poster_Path { get; set; } = "";
    public string Backdrop_Path { get; set; } = "";
    public string First_Air_Date { get; set; } = "";
    public double Vote_Average { get; set; }
    public int Vote_Count { get; set; }
    public List<int> Genre_Ids { get; set; } = new();
    public List<string> Origin_Country { get; set; } = new();
    public string Original_Language { get; set; } = "";
    public string Original_Name { get; set; } = "";
    public double Popularity { get; set; }

    public string Poster_Url { get; set; } = "";
    public string Backdrop_Url { get; set; } = "";
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
    public string Vidsrc_Embed_Url { get; set; } = "";
}

public class TvShowDetails : TvShow
{
    public int Number_Of_Seasons { get; set; }
    public int Number_Of_Episodes { get; set; }
    public string Tagline { get; set; } = "";
    public string Status { get; set; } = "";
    public List<Genre> Genres { get; set; } = new();
    public List<Season> Seasons { get; set; } = new();
    public Credits Credits { get; set; } = new();
    public Videos Videos { get; set; } = new();
    public string Vidsrc_Embed_Url { get; set; } = "";
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
    public string Profile_Path { get; set; } = "";
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
    public string Air_Date { get; set; } = "";
    public int Episode_Count { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Poster_Path { get; set; } = "";
    public int Season_Number { get; set; }
}

public class SeasonDetails
{
    public string Air_Date { get; set; } = "";
    public List<Episode> Episodes { get; set; } = new();
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public int Id { get; set; }
    public string Poster_Path { get; set; } = "";
    public int Season_Number { get; set; }
}

public class Episode
{
    public string Air_Date { get; set; } = "";
    public int Episode_Number { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Still_Path { get; set; } = "";
    public int Runtime { get; set; }
    public double Vote_Average { get; set; }
    public string Vidsrc_Embed_Url { get; set; } = "";
}

public class HealthResponse
{
    public string Status { get; set; } = "healthy";
    public string Message { get; set; } = "API is running";
}
