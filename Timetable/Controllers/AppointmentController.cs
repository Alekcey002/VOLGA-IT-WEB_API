using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Timetable.Data;
using Timetable.Models;
using Timetable.Servers;

namespace Timetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public AppointmentController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAppointment([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var json = JObject.Parse(role);
            var ids = json["id"].Value<int>();

            var timetablesToDelete = await _context.Appointments.FirstOrDefaultAsync(u => u.Id == id && (u.USerId == ids || (role.Contains("Admin") || role.Contains("Manager"))));
            if (timetablesToDelete == null)
            {
                return NotFound();
            }

            _context.Remove(timetablesToDelete);
            await _context.SaveChangesAsync();

            return Ok(timetablesToDelete);
        }
    }
}
