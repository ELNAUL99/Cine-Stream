import React, { useState, useEffect } from 'react';
import './App.css';

interface Movie {
  id: number;
  title: string;
  overview: string;
  poster_path: string;
  poster_url: string;
  backdrop_path: string;
  backdrop_url: string;
  release_date: string;
  vote_average: number;
  genre_ids: number[];
}

interface TVShow {
  id: number;
  name: string;
  overview: string;
  poster_path: string;
  poster_url: string;
  backdrop_path: string;
  backdrop_url: string;
  first_air_date: string;
  vote_average: number;
  genre_ids: number[];
}

interface MovieDetails extends Movie {
  runtime: number;
  genres: { id: number; name: string }[];
  vidsrc_embed_url: string;
  credits: {
    cast: { id: number; name: string; character: string; profile_path: string }[];
  };
}

interface TVDetails extends TVShow {
  number_of_seasons: number;
  number_of_episodes: number;
  genres: { id: number; name: string }[];
  vidsrc_embed_url: string;
  credits: {
    cast: { id: number; name: string; character: string; profile_path: string }[];
  };
}

const App: React.FC = () => {
  const [currentView, setCurrentView] = useState<'home' | 'movies' | 'tv' | 'search' | 'details'>('home');
  const [movies, setMovies] = useState<Movie[]>([]);
  const [tvShows, setTvShows] = useState<TVShow[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<(Movie | TVShow)[]>([]);
  const [selectedItem, setSelectedItem] = useState<MovieDetails | TVDetails | null>(null);
  const [loading, setLoading] = useState(false);
  const [searchType, setSearchType] = useState<'movie' | 'tv'>('movie');

  const API_BASE_URL = process.env.REACT_APP_BACKEND_URL || 'http://localhost:8001';

  useEffect(() => {
    loadPopularMovies();
    loadPopularTVShows();
  }, []);

  const loadPopularMovies = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/api/movies/popular`);
      const data = await response.json();
      setMovies(data.results || []);
    } catch (error) {
      console.error('Error loading popular movies:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadPopularTVShows = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/tv/popular`);
      const data = await response.json();
      setTvShows(data.results || []);
    } catch (error) {
      console.error('Error loading popular TV shows:', error);
    }
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchQuery.trim()) return;

    try {
      setLoading(true);
      const endpoint = searchType === 'movie' ? 'movies' : 'tv';
      const response = await fetch(`${API_BASE_URL}/api/${endpoint}/search?query=${encodeURIComponent(searchQuery)}`);
      const data = await response.json();
      setSearchResults(data.results || []);
      setCurrentView('search');
    } catch (error) {
      console.error('Error searching:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleItemClick = async (item: Movie | TVShow) => {
    try {
      setLoading(true);
      const isMovie = 'title' in item;
      const endpoint = isMovie ? 'movies' : 'tv';
      const response = await fetch(`${API_BASE_URL}/api/${endpoint}/${item.id}`);
      const data = await response.json();
      setSelectedItem(data);
      setCurrentView('details');
    } catch (error) {
      console.error('Error loading item details:', error);
    } finally {
      setLoading(false);
    }
  };

  const MovieCard: React.FC<{ movie: Movie; onClick: () => void }> = ({ movie, onClick }) => (
    <div 
      className="movie-card bg-gray-800 rounded-lg overflow-hidden hover:transform hover:scale-105 transition-all duration-300 cursor-pointer shadow-lg"
      onClick={onClick}
    >
      <div className="relative">
        <img 
          src={movie.poster_url || '/api/placeholder/300/450'} 
          alt={movie.title}
          className="w-full h-64 object-cover"
        />
        <div className="absolute top-2 right-2 bg-yellow-500 text-black px-2 py-1 rounded-full text-sm font-bold">
          {movie.vote_average.toFixed(1)}
        </div>
      </div>
      <div className="p-4">
        <h3 className="text-white font-semibold text-lg mb-2 truncate">{movie.title}</h3>
        <p className="text-gray-300 text-sm mb-2">{movie.release_date}</p>
        <p className="text-gray-400 text-sm line-clamp-2">{movie.overview}</p>
      </div>
    </div>
  );

  const TVCard: React.FC<{ show: TVShow; onClick: () => void }> = ({ show, onClick }) => (
    <div 
      className="movie-card bg-gray-800 rounded-lg overflow-hidden hover:transform hover:scale-105 transition-all duration-300 cursor-pointer shadow-lg"
      onClick={onClick}
    >
      <div className="relative">
        <img 
          src={show.poster_url || '/api/placeholder/300/450'} 
          alt={show.name}
          className="w-full h-64 object-cover"
        />
        <div className="absolute top-2 right-2 bg-yellow-500 text-black px-2 py-1 rounded-full text-sm font-bold">
          {show.vote_average.toFixed(1)}
        </div>
      </div>
      <div className="p-4">
        <h3 className="text-white font-semibold text-lg mb-2 truncate">{show.name}</h3>
        <p className="text-gray-300 text-sm mb-2">{show.first_air_date}</p>
        <p className="text-gray-400 text-sm line-clamp-2">{show.overview}</p>
      </div>
    </div>
  );

  const VideoPlayer: React.FC<{ embedUrl: string }> = ({ embedUrl }) => (
    <div className="w-full aspect-video bg-black rounded-lg overflow-hidden">
      <iframe
        src={embedUrl}
        className="w-full h-full"
        frameBorder="0"
        allowFullScreen
        title="Movie Player"
      />
    </div>
  );

  const renderHome = () => (
    <div className="space-y-8">
      {/* Hero Section */}
      <div className="hero-section bg-gradient-to-r from-purple-900 to-blue-900 rounded-lg p-8 text-center">
        <h1 className="text-4xl md:text-6xl font-bold text-white mb-4">
          🎬 CineStream
        </h1>
        <p className="text-xl text-gray-300 mb-6">
          Discover and watch the latest movies and TV shows
        </p>
        <form onSubmit={handleSearch} className="max-w-2xl mx-auto flex gap-4">
          <div className="flex-1 relative">
            <input
              type="text"
              placeholder="Search movies or TV shows..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full px-4 py-3 bg-gray-800 text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500"
            />
          </div>
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => setSearchType('movie')}
              className={`px-4 py-3 rounded-lg transition-colors ${
                searchType === 'movie' 
                  ? 'bg-purple-600 text-white' 
                  : 'bg-gray-700 text-gray-300 hover:bg-gray-600'
              }`}
            >
              Movies
            </button>
            <button
              type="button"
              onClick={() => setSearchType('tv')}
              className={`px-4 py-3 rounded-lg transition-colors ${
                searchType === 'tv' 
                  ? 'bg-purple-600 text-white' 
                  : 'bg-gray-700 text-gray-300 hover:bg-gray-600'
              }`}
            >
              TV Shows
            </button>
          </div>
          <button
            type="submit"
            className="px-6 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors"
          >
            Search
          </button>
        </form>
      </div>

      {/* Popular Movies */}
      <div>
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-white">Popular Movies</h2>
          <button
            onClick={() => setCurrentView('movies')}
            className="text-purple-400 hover:text-purple-300 transition-colors"
          >
            View All →
          </button>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {movies.slice(0, 6).map((movie) => (
            <MovieCard
              key={movie.id}
              movie={movie}
              onClick={() => handleItemClick(movie)}
            />
          ))}
        </div>
      </div>

      {/* Popular TV Shows */}
      <div>
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-white">Popular TV Shows</h2>
          <button
            onClick={() => setCurrentView('tv')}
            className="text-purple-400 hover:text-purple-300 transition-colors"
          >
            View All →
          </button>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {tvShows.slice(0, 6).map((show) => (
            <TVCard
              key={show.id}
              show={show}
              onClick={() => handleItemClick(show)}
            />
          ))}
        </div>
      </div>
    </div>
  );

  const renderMovies = () => (
    <div>
      <h2 className="text-2xl font-bold text-white mb-6">Popular Movies</h2>
      <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
        {movies.map((movie) => (
          <MovieCard
            key={movie.id}
            movie={movie}
            onClick={() => handleItemClick(movie)}
          />
        ))}
      </div>
    </div>
  );

  const renderTVShows = () => (
    <div>
      <h2 className="text-2xl font-bold text-white mb-6">Popular TV Shows</h2>
      <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
        {tvShows.map((show) => (
          <TVCard
            key={show.id}
            show={show}
            onClick={() => handleItemClick(show)}
          />
        ))}
      </div>
    </div>
  );

  const renderSearch = () => (
    <div>
      <h2 className="text-2xl font-bold text-white mb-6">
        Search Results for "{searchQuery}"
      </h2>
      <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
        {searchResults.map((item) => (
          'title' in item ? (
            <MovieCard
              key={item.id}
              movie={item}
              onClick={() => handleItemClick(item)}
            />
          ) : (
            <TVCard
              key={item.id}
              show={item}
              onClick={() => handleItemClick(item)}
            />
          )
        ))}
      </div>
    </div>
  );

  const renderDetails = () => {
    if (!selectedItem) return null;

    const isMovie = 'title' in selectedItem;
    const title = isMovie ? selectedItem.title : selectedItem.name;
    const releaseDate = isMovie ? selectedItem.release_date : selectedItem.first_air_date;

    return (
      <div className="space-y-6">
        {/* Hero Section */}
        <div 
          className="relative bg-cover bg-center rounded-lg overflow-hidden"
          style={{
            backgroundImage: `linear-gradient(rgba(0,0,0,0.7), rgba(0,0,0,0.7)), url(${selectedItem.backdrop_url || selectedItem.poster_url})`,
            minHeight: '400px'
          }}
        >
          <div className="absolute inset-0 flex items-center p-8">
            <div className="flex flex-col md:flex-row gap-6 w-full">
              <img
                src={selectedItem.poster_url || '/api/placeholder/300/450'}
                alt={title}
                className="w-64 h-96 object-cover rounded-lg shadow-2xl"
              />
              <div className="flex-1 text-white">
                <h1 className="text-4xl md:text-5xl font-bold mb-4">{title}</h1>
                <div className="flex items-center gap-4 mb-4">
                  <span className="bg-yellow-500 text-black px-3 py-1 rounded-full font-bold">
                    {selectedItem.vote_average.toFixed(1)}
                  </span>
                  <span className="text-gray-300">{releaseDate}</span>
                  {isMovie && (
                    <span className="text-gray-300">{selectedItem.runtime} min</span>
                  )}
                </div>
                <div className="flex flex-wrap gap-2 mb-4">
                  {selectedItem.genres?.map((genre) => (
                    <span
                      key={genre.id}
                      className="bg-purple-600 px-3 py-1 rounded-full text-sm"
                    >
                      {genre.name}
                    </span>
                  ))}
                </div>
                <p className="text-lg text-gray-300 mb-6 max-w-2xl">
                  {selectedItem.overview}
                </p>
                <button
                  onClick={() => {
                    const playerSection = document.getElementById('player-section');
                    playerSection?.scrollIntoView({ behavior: 'smooth' });
                  }}
                  className="bg-purple-600 hover:bg-purple-700 text-white px-8 py-3 rounded-lg font-semibold transition-colors"
                >
                  ▶ Watch Now
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Video Player */}
        <div id="player-section" className="bg-gray-800 rounded-lg p-6">
          <h2 className="text-2xl font-bold text-white mb-4">Watch {title}</h2>
          <VideoPlayer embedUrl={selectedItem.vidsrc_embed_url} />
        </div>

        {/* Cast */}
        {selectedItem.credits?.cast && (
          <div className="bg-gray-800 rounded-lg p-6">
            <h2 className="text-2xl font-bold text-white mb-4">Cast</h2>
            <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
              {selectedItem.credits.cast.slice(0, 12).map((actor) => (
                <div key={actor.id} className="text-center">
                  <img
                    src={actor.profile_path ? `https://image.tmdb.org/t/p/w185${actor.profile_path}` : '/api/placeholder/100/150'}
                    alt={actor.name}
                    className="w-full h-32 object-cover rounded-lg mb-2"
                  />
                  <p className="text-white text-sm font-semibold">{actor.name}</p>
                  <p className="text-gray-400 text-xs">{actor.character}</p>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-900">
      {/* Navigation */}
      <nav className="bg-gray-800 border-b border-gray-700">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <button
                onClick={() => setCurrentView('home')}
                className="text-2xl font-bold text-purple-400 hover:text-purple-300 transition-colors"
              >
                🎬 CineStream
              </button>
            </div>
            <div className="flex space-x-4">
              <button
                onClick={() => setCurrentView('home')}
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  currentView === 'home' 
                    ? 'bg-purple-600 text-white' 
                    : 'text-gray-300 hover:text-white hover:bg-gray-700'
                }`}
              >
                Home
              </button>
              <button
                onClick={() => setCurrentView('movies')}
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  currentView === 'movies' 
                    ? 'bg-purple-600 text-white' 
                    : 'text-gray-300 hover:text-white hover:bg-gray-700'
                }`}
              >
                Movies
              </button>
              <button
                onClick={() => setCurrentView('tv')}
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  currentView === 'tv' 
                    ? 'bg-purple-600 text-white' 
                    : 'text-gray-300 hover:text-white hover:bg-gray-700'
                }`}
              >
                TV Shows
              </button>
            </div>
          </div>
        </div>
      </nav>

      {/* Back Button */}
      {currentView !== 'home' && (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <button
            onClick={() => setCurrentView('home')}
            className="flex items-center text-purple-400 hover:text-purple-300 transition-colors"
          >
            ← Back to Home
          </button>
        </div>
      )}

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {loading && (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-500"></div>
          </div>
        )}
        
        {!loading && (
          <>
            {currentView === 'home' && renderHome()}
            {currentView === 'movies' && renderMovies()}
            {currentView === 'tv' && renderTVShows()}
            {currentView === 'search' && renderSearch()}
            {currentView === 'details' && renderDetails()}
          </>
        )}
      </main>
    </div>
  );
};

export default App;