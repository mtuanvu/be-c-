using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using five_birds_be.Models;
using Microsoft.IdentityModel.Tokens;

namespace five_birds_be.Jwt
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;


        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateJwtToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new JwtConfigurationException("JWT Key is missing or empty in configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Role", user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString()),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? GetUserIdFromHttpContext()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }



    public class JwtConfigurationException : Exception
    {
        public JwtConfigurationException(string message) : base(message) { }
    }
}
