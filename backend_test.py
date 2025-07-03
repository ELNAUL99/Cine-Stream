import requests
import unittest
import sys
import json
from datetime import datetime

class CineStreamAPITester:
    def __init__(self, base_url):
        self.base_url = base_url
        self.tests_run = 0
        self.tests_passed = 0
        self.test_results = []

    def run_test(self, name, method, endpoint, expected_status=200, params=None):
        """Run a single API test"""
        url = f"{self.base_url}/{endpoint}"
        headers = {'Content-Type': 'application/json'}
        
        self.tests_run += 1
        print(f"\n🔍 Testing {name}...")
        
        try:
            if method == 'GET':
                response = requests.get(url, params=params, headers=headers)
            elif method == 'POST':
                response = requests.post(url, json=params, headers=headers)
            
            success = response.status_code == expected_status
            
            if success:
                self.tests_passed += 1
                print(f"✅ Passed - Status: {response.status_code}")
                
                # Validate response structure
                if response.status_code == 200:
                    data = response.json()
                    self.test_results.append({
                        "name": name,
                        "status": "PASS",
                        "response_code": response.status_code,
                        "data_sample": self._get_data_sample(data)
                    })
                    return True, data
            else:
                print(f"❌ Failed - Expected {expected_status}, got {response.status_code}")
                self.test_results.append({
                    "name": name,
                    "status": "FAIL",
                    "response_code": response.status_code,
                    "error": f"Expected {expected_status}, got {response.status_code}"
                })
                return False, None

        except Exception as e:
            print(f"❌ Failed - Error: {str(e)}")
            self.test_results.append({
                "name": name,
                "status": "ERROR",
                "error": str(e)
            })
            return False, None
    
    def _get_data_sample(self, data):
        """Extract a sample of the data for reporting"""
        if isinstance(data, dict):
            if "results" in data and isinstance(data["results"], list) and len(data["results"]) > 0:
                # For paginated results, return count and first item
                return {
                    "total_results": data.get("total_results", len(data["results"])),
                    "first_item_sample": self._truncate_dict(data["results"][0])
                }
            else:
                # For single item responses
                return self._truncate_dict(data)
        return data
    
    def _truncate_dict(self, d, max_keys=5):
        """Truncate dictionary to first few keys for readability"""
        if not isinstance(d, dict):
            return d
        
        result = {}
        for i, (k, v) in enumerate(d.items()):
            if i >= max_keys:
                result["..."] = f"({len(d) - max_keys} more fields)"
                break
            result[k] = v
        return result
    
    def print_summary(self):
        """Print test results summary"""
        print("\n" + "="*50)
        print(f"📊 SUMMARY: {self.tests_passed}/{self.tests_run} tests passed")
        print("="*50)
        
        for result in self.test_results:
            status_icon = "✅" if result["status"] == "PASS" else "❌"
            print(f"{status_icon} {result['name']}")
            if result["status"] == "PASS" and "data_sample" in result:
                print(f"   Sample data: {json.dumps(result['data_sample'], indent=2)[:200]}...")
            elif result["status"] in ["FAIL", "ERROR"]:
                print(f"   Error: {result.get('error', 'Unknown error')}")
        
        return self.tests_passed == self.tests_run

class CineStreamAPITests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        # Get the backend URL from the frontend .env file
        cls.base_url = "https://bc8893c2-66bf-44ed-888b-3c6285da3cc0.preview.emergentagent.com"
        cls.tester = CineStreamAPITester(cls.base_url)
        print(f"Testing against API at: {cls.base_url}")
    
    def test_01_health_check(self):
        """Test API health endpoint"""
        success, data = self.tester.run_test(
            "API Health Check", 
            "GET", 
            "api/health"
        )
        self.assertTrue(success)
        self.assertIn("status", data)
        self.assertEqual(data["status"], "healthy")
    
    def test_02_popular_movies(self):
        """Test popular movies endpoint"""
        success, data = self.tester.run_test(
            "Popular Movies", 
            "GET", 
            "api/movies/popular"
        )
        self.assertTrue(success)
        self.assertIn("results", data)
        self.assertTrue(len(data["results"]) > 0)
        
        # Validate movie structure
        movie = data["results"][0]
        required_fields = ["id", "title", "poster_path", "poster_url"]
        for field in required_fields:
            self.assertIn(field, movie)
    
    def test_03_top_rated_movies(self):
        """Test top rated movies endpoint"""
        success, data = self.tester.run_test(
            "Top Rated Movies", 
            "GET", 
            "api/movies/top-rated"
        )
        self.assertTrue(success)
        self.assertIn("results", data)
        self.assertTrue(len(data["results"]) > 0)
    
    def test_04_movie_search(self):
        """Test movie search endpoint"""
        success, data = self.tester.run_test(
            "Movie Search", 
            "GET", 
            "api/movies/search",
            params={"query": "inception"}
        )
        self.assertTrue(success)
        self.assertIn("results", data)
        
        # Check if search returned relevant results
        if len(data["results"]) > 0:
            found_relevant = False
            for movie in data["results"]:
                if "inception" in movie["title"].lower():
                    found_relevant = True
                    break
            self.assertTrue(found_relevant, "Search did not return relevant results")
    
    def test_05_movie_details(self):
        """Test movie details endpoint"""
        # First get a movie ID from popular movies
        success, popular_data = self.tester.run_test(
            "Get Movie ID", 
            "GET", 
            "api/movies/popular"
        )
        self.assertTrue(success)
        
        if success and len(popular_data["results"]) > 0:
            movie_id = popular_data["results"][0]["id"]
            
            success, data = self.tester.run_test(
                f"Movie Details (ID: {movie_id})", 
                "GET", 
                f"api/movies/{movie_id}"
            )
            self.assertTrue(success)
            
            # Validate movie details structure
            required_fields = ["id", "title", "overview", "poster_url", "vidsrc_embed_url"]
            for field in required_fields:
                self.assertIn(field, data)
            
            # Validate VidSrc URL format
            self.assertTrue(data["vidsrc_embed_url"].startswith("https://vidsrc.to/embed/movie/"))
    
    def test_06_popular_tv_shows(self):
        """Test popular TV shows endpoint"""
        success, data = self.tester.run_test(
            "Popular TV Shows", 
            "GET", 
            "api/tv/popular"
        )
        self.assertTrue(success)
        self.assertIn("results", data)
        self.assertTrue(len(data["results"]) > 0)
        
        # Validate TV show structure
        show = data["results"][0]
        required_fields = ["id", "name", "poster_path", "poster_url"]
        for field in required_fields:
            self.assertIn(field, show)
    
    def test_07_tv_search(self):
        """Test TV show search endpoint"""
        success, data = self.tester.run_test(
            "TV Show Search", 
            "GET", 
            "api/tv/search",
            params={"query": "breaking bad"}
        )
        self.assertTrue(success)
        self.assertIn("results", data)
    
    def test_08_tv_details(self):
        """Test TV show details endpoint"""
        # First get a TV show ID from popular shows
        success, popular_data = self.tester.run_test(
            "Get TV Show ID", 
            "GET", 
            "api/tv/popular"
        )
        self.assertTrue(success)
        
        if success and len(popular_data["results"]) > 0:
            tv_id = popular_data["results"][0]["id"]
            
            success, data = self.tester.run_test(
                f"TV Show Details (ID: {tv_id})", 
                "GET", 
                f"api/tv/{tv_id}"
            )
            self.assertTrue(success)
            
            # Validate TV show details structure
            required_fields = ["id", "name", "overview", "poster_url", "vidsrc_embed_url"]
            for field in required_fields:
                self.assertIn(field, data)
            
            # Validate VidSrc URL format
            self.assertTrue(data["vidsrc_embed_url"].startswith("https://vidsrc.to/embed/tv/"))
    
    def test_09_tv_season_details(self):
        """Test TV show season details endpoint"""
        # First get a TV show ID from popular shows
        success, popular_data = self.tester.run_test(
            "Get TV Show ID for Season", 
            "GET", 
            "api/tv/popular"
        )
        self.assertTrue(success)
        
        if success and len(popular_data["results"]) > 0:
            tv_id = popular_data["results"][0]["id"]
            
            success, data = self.tester.run_test(
                f"TV Show Season Details (ID: {tv_id}, Season: 1)", 
                "GET", 
                f"api/tv/{tv_id}/season/1"
            )
            self.assertTrue(success)
            
            # Validate season details structure
            self.assertIn("episodes", data)
            
            # Validate episode structure if episodes exist
            if len(data["episodes"]) > 0:
                episode = data["episodes"][0]
                required_fields = ["episode_number", "name", "vidsrc_embed_url"]
                for field in required_fields:
                    self.assertIn(field, episode)

if __name__ == "__main__":
    # Run the tests
    unittest.main(argv=['first-arg-is-ignored'], exit=False)
    
    # Print summary
    tester = CineStreamAPITests.tester
    success = tester.print_summary()
    
    # Exit with appropriate code
    sys.exit(0 if success else 1)