using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly Dictionary<string, string> Users = new();
        private readonly string _jwtSecret = "your-very-strong-secret-key-1234567890";
        private readonly string _jwtIssuer = "AppBackend";
        private readonly string _jwtAudience = "AppBackendUsers";

        [HttpPost("signin")]
        public IActionResult SignIn([FromBody] UserCredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("username and password are required");

            if (Users.ContainsKey(credentials.Username))
                return Conflict("user already exists");

            Users[credentials.Username] = credentials.Password;
            return Created($"/api/users/{credentials.Username}", new { credentials.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("username and password are required");

            if (!Users.TryGetValue(credentials.Username, out var storedPassword) || storedPassword != credentials.Password)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, credentials.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenString, token_type = "Bearer", expires_in = 3600 });
        }

        [Authorize]
        [HttpGet("secure")]
        public IActionResult SecureEndpoint()
        {
            return Ok("secure area: access granted");
        }
    }

    public class UserCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
