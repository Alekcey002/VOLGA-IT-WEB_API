using Document.Data;
using Document.Models;
using Document.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Document.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public HistoryController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }

        [HttpGet("Account/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAccountDocumen([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("User") && !role.Contains("Doctor"))
            {
                return Unauthorized();
            }

            var json = JObject.Parse(role);
            var ids = json["id"].Value<int>();

            var userGet = await _context.Documents.Where(u => u.PacientId == id && ((u.PacientId == ids && role.Contains("User")) || (u.DoctorId == ids && role.Contains("Doctor")))).ToListAsync();
            if (userGet == null)
            {
                return NotFound();
            }

            return Ok(userGet);

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> DocumentsDataGetId([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("User") && !role.Contains("Doctor"))
            {
                return Unauthorized();
            }

            var json = JObject.Parse(role);
            var ids = json["id"].Value<int>();

            var userGet = await _context.Documents.Where(u => u.Id == id && ((u.PacientId == ids && role.Contains("User")) || (u.DoctorId == ids && role.Contains("Doctor")))).ToListAsync();
            if (userGet == null)
            {
                return NotFound();
            }

            return Ok(userGet);
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> DocumentsDataPost([FromBody] DocumentDb document)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var role = await _authenticationService.GetPacientAsync(token);

            if (role == null)
            {
                return Unauthorized();
            }

            var newDocumentData = new DocumentDb
            {
                Date = document.Date,
                PacientId = document.PacientId,
                HospitalId = document.HospitalId,
                DoctorId = document.DoctorId,
                Room = document.Room,
                Data = document.Data
            };

            var userRole = role.FirstOrDefault(r => r.Id == document.PacientId && r.Roles.Contains("User"));
            if (userRole != null)
            {
                var hospital = await _authenticationService.GetHospitalAsync(document.HospitalId, token);
                if (hospital == document.HospitalId)
                {
                    var hospitalRoom = await _authenticationService.GetRoomAsync(document.HospitalId, token);
                    if (hospitalRoom != null && hospitalRoom.Contains($"{document.Room}"))
                    {
                        var doctor = await _authenticationService.GetDoctorAsync(document.DoctorId, token);
                        if (doctor == document.DoctorId)
                        {
                            _context.Add(newDocumentData);
                            _context.SaveChanges();

                            return Ok(newDocumentData);
                        }

                        return NotFound("Нет такого доктора");
                    }

                    return NotFound("Нет такой кабинеты");
                }

                return NotFound("Нет такого больницы");
            }

            return NotFound("Нет такого пользователя");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> DocumentsDataPut([FromBody] DocumentDb document, [FromRoute] int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var role = await _authenticationService.GetPacientAsync(token);

            if (role == null)
            {
                return Unauthorized();
            }

            var documentPut = await _context.Documents.FirstOrDefaultAsync(u => u.Id == id);
            if (documentPut == null)
            {
                return NotFound();
            }

            documentPut.Date = document.Date;
            documentPut.PacientId = document.PacientId;
            documentPut.HospitalId = document.HospitalId;
            documentPut.DoctorId = document.DoctorId;
            documentPut.Data = document.Data;


            var userRole = role.FirstOrDefault(r => r.Id == document.PacientId && r.Roles.Contains("User"));
            if (userRole != null)
            {
                var hospital = await _authenticationService.GetHospitalAsync(document.HospitalId, token);
                if (hospital == document.HospitalId)
                {
                    var hospitalRoom = await _authenticationService.GetRoomAsync(document.HospitalId, token);
                    if (hospitalRoom != null && hospitalRoom.Contains($"{document.Room}"))
                    {
                        var doctor = await _authenticationService.GetDoctorAsync(document.DoctorId, token);
                        if (doctor == document.DoctorId)
                        {
                            _context.Update(documentPut);
                            _context.SaveChanges();

                            return Ok(documentPut);
                        }

                        return NotFound($"Нет такого доктора");
                    }

                    return NotFound("Нет такой кабинеты");
                }

                return NotFound("Нет такого больницы");
            }

            return NotFound("Нет такого пользователя");
        }
    }
}
