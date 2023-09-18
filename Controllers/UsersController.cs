using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using server.Models.Dtos;

namespace server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly string key;
        public UsersController(EcommerceDbContext context, IConfiguration config)
        {
            _context = context;
            key = config.GetSection("Settings").GetSection("Key").ToString();
        }
        [HttpPost]
        [Route("register-user")]
        public IActionResult RegisterUser([FromBody] RegisterUserDto user)
        {
            if(user.UserEmail == "" || user.UserUsername== "" || user.UserPassword == "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "The info cannot be empty" });
            }
            var emailAlreadyExists = _context.Users.FirstOrDefault(cd => cd.UserEmail == user.UserEmail);
            var usernameAlreadyExists = _context.Users.FirstOrDefault(cd => cd.UserUsername == user.UserUsername);
            if (emailAlreadyExists != null)
            {
                return StatusCode(StatusCodes.Status203NonAuthoritative, new { message = "Email already registered" });
            }
            else if (usernameAlreadyExists != null)
            {
                return StatusCode(StatusCodes.Status203NonAuthoritative, new { message = "Username already registered" });

            }else if(emailAlreadyExists == null && usernameAlreadyExists == null)
            {
                var newUser = new User
                {
                    UserUsername = user.UserUsername,
                    UserEmail = user.UserEmail,
                    UserPassword = user.UserPassword,
                };
             _context.Users.Add(newUser);

            }
             _context.SaveChanges();
            return StatusCode(StatusCodes.Status200OK, new {message="User registered correctly"});

        }
        [HttpPost]
        [Route("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto user)
        {
            var userIsValid = await _context.Users.FirstOrDefaultAsync(c=> c.UserEmail == user.UserEmail && c.UserPassword == user.UserPassword);
            if (userIsValid == null)
            {
                return StatusCode(StatusCodes.Status203NonAuthoritative, new {userData = "", Message = "Incorrect data" });
            }
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserEmail));
            claims.AddClaim(new Claim(ClaimTypes.Role, "user"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string tokenCreado = tokenHandler.WriteToken(tokenConfig);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/"
            };
            LoggedUserResponseDto userValid = new()
            {
                UserId = userIsValid.UserId,
                UserEmail = userIsValid.UserEmail,
                UserUsername = userIsValid.UserUsername,
            };
            Response.Cookies.Append("sessionToken", tokenCreado, cookieOptions);
            return StatusCode(StatusCodes.Status200OK, new {userData = userValid, tokenExpires=DateTime.UtcNow.AddDays(1)});
        }
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
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
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.ToString() });
            }
        }
    }

}
