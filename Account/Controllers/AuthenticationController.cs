using Account.Data;
using Account.Models;
using Account.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(DbContextOptions<DataContext> options, IConfiguration builder, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _configuration = builder;
            _authenticationService = authenticationService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUp user)
        {
            var newUser = new SignUp
            {
                LastName = user.LastName,
                FirstName = user.FirstName,
                Username = user.Username,
                Password = user.Password
            };

            _context.Add(newUser);
            _context.SaveChanges();

            return Ok(newUser);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] User model)
        {
            var person = _context.SignUps.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if (person == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, model.Username),
                };

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                notBefore: now,
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddDays(1);

            var newToken = new SignIn
            {
                Tokens = encodedJwt,
                RefreshToken = refreshToken,
                Username = model.Username,
                Expiry = expiry
            };

            _context.Add(newToken);
            _context.SaveChanges();

            return Ok(new { AccessToken = encodedJwt, RefreshToken = refreshToken });
        }

        [HttpGet("Validate")]
        public async Task<IActionResult> ValidateToken([FromQuery] string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
                };

                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
                var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                var userGet = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);

                if (userGet != null)
                {
                    return Ok(new { Id = userGet.Id, Username = userGet.Username, Roles = userGet.Roles });
                }
                return Ok(new { Username = username, Roles = "NoUser" });
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            var tokenInDb = _context.SignIns.FirstOrDefault(x => x.RefreshToken == refreshToken.refreshToken);
            if (tokenInDb == null || tokenInDb.Expiry < DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, tokenInDb.Username),
            };

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                notBefore: now,
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var PutRefreshToken = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddDays(1);

            var newToken = new SignIn
            {
                Tokens = encodedJwt,
                RefreshToken = PutRefreshToken,
                Username = tokenInDb.Username,
                Expiry = expiry
            };

            _context.Add(newToken);
            _context.SaveChanges();

            return Ok(new { AccessToken = encodedJwt, RefreshToken = PutRefreshToken });
        }

        [HttpPut("SignOut")]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            var username = await _authenticationService.GetUsernameAsync(HttpContext);
            var userPut = await _context.SignIns.FirstOrDefaultAsync(u => u.Username == username);
            if (userPut == null)
            {
                return Unauthorized();
            }

            userPut.Tokens = "Null";
            userPut.RefreshToken = "Null";
            await _context.SaveChangesAsync();

            return Ok("Успешно вышли из аккаунта");
        }
    }
}