using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Services;

namespace Movies.Controllers
{
    // Api controller for user authentication
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : ControllerBase
    {
        // Authentication service instance
        private readonly IAuth _authService;

        //authentication service injection
        public Auth(IAuth authService)
        {
            _authService = authService;
        }

        // user registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequest request)
        {
            try
            {
                // register user with provided details
                var user = await _authService.Register(request.Username, request.Password, request.Email);
                var token = _authService.GenerateJWTToken(user);

                // return authentication response with token
                return Ok(new AuthenticationResponse
                {
                    Token = token,
                    Username = user.Username,
                    Expires = DateTime.Now.AddHours(2)
                });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // user login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            try
            {
                // authenticate user and generate token
                var token = await _authService.Login(request.Username, request.Password);

                // return authentication response with token
                return Ok(new AuthenticationResponse
                {
                    Token = token,
                    Username = request.Username,
                    Expires = DateTime.Now.AddHours(2)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
