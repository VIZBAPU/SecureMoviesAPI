namespace Movies.Models
{
    // Model for authentication request
    public class AuthenticationRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Model for authentication response
    public class AuthenticationResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }
}
