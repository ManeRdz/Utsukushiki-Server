using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Models;
using server.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly string key;

        public AdminsController(EcommerceDbContext context, IConfiguration config)
        {
            _context = context;
            key = config.GetSection("Settings").GetSection("Key").ToString();
        }

        [HttpPost]
        [Route("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto request)
        {
            var validAdminUser = await _context.Admins.FirstOrDefaultAsync(c=> c.AdminUsername == request.AdminUsername && c.AdminPassword == request.AdminPassword);
            if (validAdminUser == null)
            {
                return StatusCode(StatusCodes.Status203NonAuthoritative, new { message = "Invalid data", tokenExpires = "" });
            }
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, validAdminUser.AdminUsername));
            claims.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string tokenCreado = tokenHandler.WriteToken(tokenConfig);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/"
            };
            Response.Cookies.Append("sessionToken", tokenCreado, cookieOptions);
            return StatusCode(StatusCodes.Status200OK, new {message="Logged in",tokenExpires = DateTime.UtcNow.AddMinutes(30)});
        }
        [HttpPost]
        [Route("admin-logout")]
        public IActionResult AdminLogout()
        {
            try
            {
                string cookieName = "sessionToken";
                if (Request.Cookies.ContainsKey(cookieName))
                {

                    Response.Cookies.Append(cookieName, Request.Cookies[cookieName], new CookieOptions
                    {
                        Expires = DateTime.UtcNow,
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true,
                        Path = "/"
                    });
                    return StatusCode(StatusCodes.Status200OK, new { message = "Logged out" });
                }
                else
                {

                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "You're already logged out" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.ToString() });
            }
        }
    }
}
