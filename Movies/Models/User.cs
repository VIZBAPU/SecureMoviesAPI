using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;


namespace Movies.Models
{
    // User model for MongoDB
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Username is mandatory")]
        public string Username { get; set; } = string.Empty;

        // Hashed password
        public byte[] PasswordHash { get; set; } = new byte[0];
        
        // Salt used for hashing the password
        public byte[] PasswordSalt { get; set; } = new byte[0];

        [Required(ErrorMessage = "Email is mandatory")]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
