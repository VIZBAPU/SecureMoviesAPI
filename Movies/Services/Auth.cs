using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Movies.Models;
using MongoDB.Driver;

namespace Movies.Services
{
    
    public class Auth : IAuth
    {

        // MongoDB collection for users
        private readonly IMongoCollection<User> _users;
        
        // Configuration for JWT settings
        private readonly IConfiguration _configuration;

        // Auth service constructor
        public Auth(IMongoDatabase database, IConfiguration configuration)
        {
            _users = database.GetCollection<User>("Users");
            _configuration = configuration;
        }

        // Register a new user
        public async Task<User> Register(string username, string password, string email)
        {
            if (await _users.Find(u => u.Username == username).AnyAsync())
                throw new Exception("Username already exists");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _users.InsertOneAsync(user);
            return user;
        }

        // User login method
        public async Task<string> Login(string username, string password)
        {
            Console.WriteLine($"Login attempt for user: {username}");

            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine("User not found in database");
                throw new Exception("User not found");
            }

            Console.WriteLine($"User found: {user.Username}, checking password...");

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                Console.WriteLine("Password verification failed");
                throw new Exception("Incorrect password");
            }

            Console.WriteLine("Login successful, generating token");
            return GenerateJWTToken(user);
        }

        // Generate JWT token for authenticated user
        public string GenerateJWTToken(User user)
        {
            // Define user claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id!)
            };

            // Create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Use HMACSHA512 to create a password hash and salt
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Password verification method
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
