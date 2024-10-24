using Account.Data;
using Account.Models;
using Account.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Account.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public AccountsController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> MeUserData()
        {
            var username = await _authenticationService.GetUsernameAsync(HttpContext);
            var userGet = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);
            if (userGet == null)
            {
                return Unauthorized();
            }

            return Ok(userGet);
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> UserDataPut([FromBody] UserUpdate userDataUpdate)
        {
            var username = await _authenticationService.GetUsernameAsync(HttpContext);
            var userPut = await _context.SignUps.FirstOrDefaultAsync(u => u.Username == username);
            if (userPut == null)
            {
                return Unauthorized();
            }

            userPut.FirstName = userDataUpdate.FirstName;
            userPut.LastName = userDataUpdate.LastName;
            userPut.Password = userDataUpdate.Password;

            _context.Update(userPut);
            await _context.SaveChangesAsync();

            return Ok(userPut);
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> ActionDataGet([FromQuery] int from = 0, [FromQuery] int count = 100)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (!role.Contains("Admin") && !HttpContext.Request.Headers["Origin"].ToString().Contains("http://localhost:8002"))
            {
                return Unauthorized();
            }

            if (HttpContext.Request.Headers["Origin"].ToString().Contains("http://localhost:8002"))
            {
                if (role.Contains("Admin") || role.Contains("Doctor") || role.Contains("Manager"))
                {
                    var accounts = _context.Accounts
                        .Skip(from)
                        .Take(count)
                        .Select(u => new { u.Id, u.FirstName, u.LastName, u.Username, u.Roles })
                        .ToList();

                    return Ok(accounts);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                var accounts = _context.Accounts
                    .Skip(from)
                    .Take(count)
                    .Select(u => new { u.Id, u.FirstName, u.LastName, u.Username, u.Roles })
                    .ToList(); 
                return Ok(accounts);
            }
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> ActionDataPost([FromBody] AccountDb userData)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var newUserData = new AccountDb
            {
                LastName = userData.LastName,
                FirstName = userData.FirstName,
                Username = userData.Username,
                Password = userData.Password,
                Roles = userData.Roles
            };

            _context.Add(newUserData);
            _context.SaveChanges();

            return Ok(newUserData);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ActionDataPut([FromBody] AccountDb userData , [FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var userPut = await _context.Accounts.FirstOrDefaultAsync(u => u.Id == id);
            if (userPut == null)
            {
                return NotFound();
            }

            userPut.LastName = userData.LastName;
            userPut.FirstName = userData.FirstName;
            userPut.Username = userData.Username;
            userPut.Password = userData.Password;
            userPut.Roles = userData.Roles;

            _context.Update(userPut);
            await _context.SaveChangesAsync();

            return Ok(userPut);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> ActionDataDelete([FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var userDel = await _context.Accounts.FirstOrDefaultAsync(u => u.Id == id);
            if (userDel == null)
            {
                return NotFound();
            }

            _context.Remove(userDel);
            await _context.SaveChangesAsync();

            return Ok(userDel);
        }
    }
}