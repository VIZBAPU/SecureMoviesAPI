using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Movie
    {
        // Unique identifier for the movie
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Movie title is mandatory")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Director name is mandatory")]
        [StringLength(50, ErrorMessage = "Director name can't be longer than 50 characters")]
        public string Director { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release year is mandatory")]
        [Range(1900, 2030, ErrorMessage = "Release year must be between 1900 and 2030")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Genre is mandatory")]
        [StringLength(30, ErrorMessage = "Genre can't be longer than 30 characters")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rating is mandatory")]
        [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public double Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;

    }
}
