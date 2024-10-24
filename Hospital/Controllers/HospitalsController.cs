using Hospital.Data;
using Hospital.Models;
using Hospital.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public HospitalsController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataGet([FromQuery] int from = 0, [FromQuery] int count = 100)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var hospitals = _context.Hospitals
                .Skip(from)
                .Take(count)
                .Select(u => new { u.Id, u.Name, u.Address, u.ContactPhone })
                .ToList();

            return Ok(hospitals);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataGetId([FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var hospital = await _context.Hospitals.FirstOrDefaultAsync(u => u.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return Ok(new { hospital.Id, hospital.Name, hospital.Address, hospital.ContactPhone });
        }

        [HttpGet("{id}/Rooms")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataGetRooms([FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var hospital = await _context.Hospitals.FirstOrDefaultAsync(u => u.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return Ok(new { hospital.Rooms });
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataPost([FromBody] HospitalDb hospital)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);

            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var newHospitalData = new HospitalDb
            {
                Name = hospital.Name,
                Address = hospital.Address,
                ContactPhone = hospital.ContactPhone,
                Rooms = hospital.Rooms
            };

            _context.Add(newHospitalData);
            _context.SaveChanges();

            return Ok(newHospitalData);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataPut([FromBody] HospitalDb hospital, [FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);

            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var hospitalPut = await _context.Hospitals.FirstOrDefaultAsync(u => u.Id == id);
            if (hospitalPut == null)
            {
                return NotFound();
            }

            hospitalPut.Name = hospital.Name;
            hospitalPut.Address = hospital.Address;
            hospitalPut.ContactPhone = hospital.ContactPhone;
            hospitalPut.Rooms = hospital.Rooms;

            _context.Update(hospitalPut);
            await _context.SaveChangesAsync();

            return Ok(hospitalPut);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> HospitalsDataDelete([FromRoute] int id)
        {
            var role = await _authenticationService.GetRoleAsync(HttpContext);

            if (!role.Contains("Admin"))
            {
                return Unauthorized();
            }

            var hospitalDel = await _context.Hospitals.FirstOrDefaultAsync(u => u.Id == id);
            if (hospitalDel == null)
            {
                return NotFound();
            }

            _context.Remove(hospitalDel);
            await _context.SaveChangesAsync();

            return Ok(hospitalDel);
        }
    }
}
