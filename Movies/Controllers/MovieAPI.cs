using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Movies.Models;

namespace Movies.Controllers
{
    // API controller connected to MongoDB database
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovieAPI : ControllerBase
    {
        // MongoDB collection for movies
        private readonly IMongoCollection<Movie> _movies;
        public MovieAPI(IMongoDatabase database)
        {
            _movies = database.GetCollection<Movie>("Movies");
        }

        // GET: api/MovieAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _movies.Find(_ => true).ToListAsync();
            return movies;
        }
        
        // GET: api/MovieAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(string id)
        {
            var movie = await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }
        
        // POST: api/MovieAPI
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            movie.CreatedBy = User.Identity?.Name ?? "APIUser";
            movie.CreatedAt = DateTime.UtcNow;
            await _movies.InsertOneAsync(movie);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }
        
        // PUT: api/MovieAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(string id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            var existingMovie = await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (existingMovie == null)
            {
                return NotFound();
            }

            await _movies.ReplaceOneAsync(m => m.Id == id, movie);
            return NoContent();
        }
        
        // DELETE: api/MovieAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(string id)
        {
            var result= await _movies.DeleteOneAsync(m => m.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
        private bool MovieExists(string id)
        {
            return _movies.Find(m => m.Id == id).Any();
        }   
    }
}
