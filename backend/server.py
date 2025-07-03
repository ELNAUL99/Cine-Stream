from fastapi import FastAPI, HTTPException, Query
from fastapi.middleware.cors import CORSMiddleware
from pymongo import MongoClient
import os
import httpx
from typing import Optional
import logging

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(title="Filming Website API")

# CORS configuration
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# MongoDB connection
MONGO_URL = os.environ.get('MONGO_URL', 'mongodb://localhost:27017/')
client = MongoClient(MONGO_URL)
db = client.filming_website

# TMDb API configuration
TMDB_API_KEY = os.environ.get('TMDB_API_KEY', '51ed980914e44c3876fa3cd1271216d1')
TMDB_BASE_URL = "https://api.themoviedb.org/3"
TMDB_IMAGE_BASE_URL = "https://image.tmdb.org/t/p/w500"

@app.get("/")
async def root():
    return {"message": "Filming Website API is running!"}

@app.get("/api/health")
async def health_check():
    return {"status": "healthy", "message": "API is running"}

@app.get("/api/movies/popular")
async def get_popular_movies(page: int = Query(1, ge=1, le=500)):
    """Get popular movies from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/movie/popular",
                params={"api_key": TMDB_API_KEY, "page": page}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            for movie in data.get("results", []):
                if movie.get("poster_path"):
                    movie["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{movie['poster_path']}"
                if movie.get("backdrop_path"):
                    movie["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{movie['backdrop_path']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching popular movies")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/movies/top-rated")
async def get_top_rated_movies(page: int = Query(1, ge=1, le=500)):
    """Get top rated movies from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/movie/top_rated",
                params={"api_key": TMDB_API_KEY, "page": page}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            for movie in data.get("results", []):
                if movie.get("poster_path"):
                    movie["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{movie['poster_path']}"
                if movie.get("backdrop_path"):
                    movie["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{movie['backdrop_path']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching top rated movies")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/movies/search")
async def search_movies(query: str = Query(..., min_length=1), page: int = Query(1, ge=1, le=500)):
    """Search movies from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/search/movie",
                params={"api_key": TMDB_API_KEY, "query": query, "page": page}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            for movie in data.get("results", []):
                if movie.get("poster_path"):
                    movie["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{movie['poster_path']}"
                if movie.get("backdrop_path"):
                    movie["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{movie['backdrop_path']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error searching movies")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/movies/{movie_id}")
async def get_movie_details(movie_id: int):
    """Get movie details from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/movie/{movie_id}",
                params={"api_key": TMDB_API_KEY, "append_to_response": "credits,videos"}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            if data.get("poster_path"):
                data["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{data['poster_path']}"
            if data.get("backdrop_path"):
                data["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{data['backdrop_path']}"
            
            # Add VidSrc embed URL
            data["vidsrc_embed_url"] = f"https://vidsrc.me/embed/movie/{movie_id}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching movie details")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/tv/popular")
async def get_popular_tv_shows(page: int = Query(1, ge=1, le=500)):
    """Get popular TV shows from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/tv/popular",
                params={"api_key": TMDB_API_KEY, "page": page}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            for show in data.get("results", []):
                if show.get("poster_path"):
                    show["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{show['poster_path']}"
                if show.get("backdrop_path"):
                    show["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{show['backdrop_path']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching popular TV shows")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/tv/search")
async def search_tv_shows(query: str = Query(..., min_length=1), page: int = Query(1, ge=1, le=500)):
    """Search TV shows from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/search/tv",
                params={"api_key": TMDB_API_KEY, "query": query, "page": page}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            for show in data.get("results", []):
                if show.get("poster_path"):
                    show["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{show['poster_path']}"
                if show.get("backdrop_path"):
                    show["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{show['backdrop_path']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error searching TV shows")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/tv/{tv_id}")
async def get_tv_details(tv_id: int):
    """Get TV show details from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/tv/{tv_id}",
                params={"api_key": TMDB_API_KEY, "append_to_response": "credits,videos"}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add full image URLs
            if data.get("poster_path"):
                data["poster_url"] = f"{TMDB_IMAGE_BASE_URL}{data['poster_path']}"
            if data.get("backdrop_path"):
                data["backdrop_url"] = f"https://image.tmdb.org/t/p/w1280{data['backdrop_path']}"
            
            # Add VidSrc embed URL
            data["vidsrc_embed_url"] = f"https://vidsrc.to/embed/tv/{tv_id}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching TV show details")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

@app.get("/api/tv/{tv_id}/season/{season_number}")
async def get_tv_season_details(tv_id: int, season_number: int):
    """Get TV show season details from TMDb"""
    try:
        async with httpx.AsyncClient() as client:
            response = await client.get(
                f"{TMDB_BASE_URL}/tv/{tv_id}/season/{season_number}",
                params={"api_key": TMDB_API_KEY}
            )
            response.raise_for_status()
            data = response.json()
            
            # Add VidSrc embed URLs for episodes
            for episode in data.get("episodes", []):
                episode["vidsrc_embed_url"] = f"https://vidsrc.me/embed/tv/{tv_id}/{season_number}/{episode['episode_number']}"
            
            return data
    except httpx.HTTPStatusError as e:
        logger.error(f"TMDb API error: {e}")
        raise HTTPException(status_code=500, detail="Error fetching TV season details")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        raise HTTPException(status_code=500, detail="Internal server error")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)