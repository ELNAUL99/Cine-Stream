# Cine Stream Backend - C# / ASP.NET Core

Full C# rewrite of the Python FastAPI backend with the same API endpoints.

## Tech Stack

- **ASP.NET Core 8** - Web framework
- **TMDb API** - Movie/TV show data
- **Swashbuckle** - OpenAPI/Swagger docs
- **Docker** - Containerization

## Project Structure

```
backend-csharp/
├── Controllers/
│   ├── MoviesController.cs      # GET /api/movies/*
│   ├── TvShowsController.cs     # GET /api/tv/*
│   ├── HealthController.cs      # GET /api/health
│   └── RootController.cs        # GET /
├── Models/
│   └── Movie.cs                 # All data models
├── Services/
│   └── TmdbService.cs           # TMDb API client
├── appsettings.json             # Configuration
├── CineStream.csproj            # Project file
├── Dockerfile                   # Docker build
├── railway.json                 # Railway config
└── Program.cs                   # Entry point
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | API status |
| GET | `/api/health` | Health check |
| GET | `/api/movies/popular` | Popular movies |
| GET | `/api/movies/top-rated` | Top rated movies |
| GET | `/api/movies/search?query=` | Search movies |
| GET | `/api/movies/{id}` | Movie details |
| GET | `/api/tv/popular` | Popular TV shows |
| GET | `/api/tv/search?query=` | Search TV shows |
| GET | `/api/tv/{id}` | TV show details |
| GET | `/api/tv/{id}/season/{num}` | Season details |

## Local Development

```bash
cd backend-csharp

# Set environment variable (or add to appsettings.json)
export Tmdb__ApiKey="your-tmdb-api-key"

# Run
dotnet run

# API will be at http://localhost:5250 (or similar)
# Swagger docs at http://localhost:5250/swagger
```

## Deploy to Railway

1. Create a **new repo** with just the `backend-csharp/` folder contents
2. Push to GitHub
3. Go to [railway.app](https://railway.app) → New Project → Deploy from GitHub
4. Railway will auto-detect the `railway.json` and Dockerfile
5. Add environment variable:
   ```
   Tmdb__ApiKey = your-tmdb-api-key
   ```
6. Done! Your API will be at `https://your-app.up.railway.app`

## Deploy to Render

1. Go to [render.com](https://render.com) → New + → Web Service
2. Connect your GitHub repo
3. Configure:
   - **Runtime:** Docker
   - **Dockerfile Path:** `Dockerfile`
4. Add environment variable:
   ```
   Tmdb__ApiKey = your-tmdb-api-key
   ```
5. Click Create Web Service

## Deploy to Fly.io

```bash
# Install flyctl (if not already)
brew install flyctl

# Login
flyctl auth login

# Launch (from backend-csharp directory)
cd backend-csharp
flyctl launch

# Set secrets
flyctl secrets set Tmdb__ApiKey=your-tmdb-api-key

# Deploy
flyctl deploy
```

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `Tmdb__ApiKey` | Yes | Your TMDb API key |
| `Tmdb__BaseUrl` | No | TMDb base URL (default: https://api.themoviedb.org/3) |
| `Cors__AllowedOrigins__0` | No | Frontend URL for CORS |

## Why C# over Python?

| Feature | C# (this) | Python (original) |
|---------|-----------|-------------------|
| **Performance** | ~3x faster | Slower |
| **Type Safety** | Strong types | Dynamic (runtime errors) |
| **Async** | Native async/await | asyncio |
| **Deployment** | Single binary | Need Python runtime |
| **Memory** | Lower footprint | Higher |
| **IDE Support** | Excellent (Rider/VS) | Good |

## Docker Build

```bash
cd backend-csharp

# Build
docker build -t cine-stream-api .

# Run
docker run -p 8080:8080 -e Tmdb__ApiKey=your-key cine-stream-api
```
