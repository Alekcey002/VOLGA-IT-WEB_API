using Account.Data;
using Account.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public DoctorsController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }
        
        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> DoctorDataGet([FromQuery] string? nameFilter, [FromQuery] int from = 0, [FromQuery] int count = 100)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var accountsQuery = _context.Accounts.AsQueryable();
            if (!string.IsNullOrEmpty(nameFilter))
            {
                accountsQuery = accountsQuery.Where(a => a.FirstName.Contains(nameFilter));
            }

            var doctors = await accountsQuery
                .Where(u => u.Roles.Contains("Doctor"))
                .Skip(from)
                .Take(count)
                .Select(u => new { u.Id, u.FirstName, u.LastName})
                .ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> DoctorDataGetId([FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var doctor = await _context.Accounts.FirstOrDefaultAsync(u => u.Id == id && u.Roles.Contains("Doctor"));
            if (doctor == null)
            {
                return NotFound();
            }

            return Ok( new { doctor.Id, doctor.FirstName, doctor.LastName });
        }
    }
}
