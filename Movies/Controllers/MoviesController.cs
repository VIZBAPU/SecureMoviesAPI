using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Movies.Models;
using System.Linq;

namespace Movies.Controllers
{
    public class MoviesController : Controller
    {
        // MongoDB collection for movies
        private readonly IMongoCollection<Movie> _movies;

        public MoviesController(IMongoDatabase database)
        {
            _movies = database.GetCollection<Movie>("Movies");
        }

        // Return pre included movies with sorting functionality
        public async Task<IActionResult> Index(string sortBy)
        {
            var movies = await _movies.Find(_ => true).ToListAsync();
            var moviesList = movies.AsEnumerable();

            switch (sortBy?.ToLower())
            {
                case "rating":
                    moviesList = moviesList.OrderByDescending(m => m.Rating);
                    break;
                case "rating_asc":
                    moviesList = moviesList.OrderBy(m => m.Rating);
                    break;
                case "year":
                    moviesList = moviesList.OrderByDescending(m => m.ReleaseYear);
                    break;
                case "year_asc":
                    moviesList = moviesList.OrderBy(m => m.ReleaseYear);
                    break;
                case "title":
                    moviesList = moviesList.OrderBy(m => m.Title);
                    break;
                case "title_desc":
                    moviesList = moviesList.OrderByDescending(m => m.Title);
                    break;
                default:
                    moviesList = moviesList.OrderBy(m => m.Id);
                    break;
            }

            ViewBag.SortBy = sortBy;
            return View(moviesList.ToList());
        }

        // View movie details
        public IActionResult Create()
        {
            return View();
        }

        // Add a new movie
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.CreatedBy = "WebUser";
                movie.CreatedAt = DateTime.UtcNow;
                await _movies.InsertOneAsync(movie);
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // View certain movie details to edit
        public async Task<IActionResult> Edit(string id)
        {
            var movie = await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();        // Data fetched using same Id
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // Edit a movie
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _movies.ReplaceOneAsync(m => m.Id == id, movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // View certain movie details to delete
        public async Task<IActionResult> Delete(string id)
        {
            var movie = await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();                // Data fetched using same Id
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // Delete a movie
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movie = await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (movie == null)
            {
                return NotFound();
            }                                             // delete the movie
            return RedirectToAction(nameof(Index));
        }
    }
}
