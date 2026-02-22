using Movies.Models;

namespace Movies.Services
{
    public interface IAuth
    {
        // Register a new user
        Task<User> Register(string username, string password, string email);

        // User login method
        Task<string> Login(string username, string password);

        // Generate JWT token for authenticated user
        string GenerateJWTToken(User user);
    }
}
