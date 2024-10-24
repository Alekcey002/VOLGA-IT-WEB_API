using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Xml;
using Timetable.Data;
using Timetable.Models;
using Timetable.Servers;

namespace Timetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthenticationService _authenticationService;

        public TimetableController(DbContextOptions<DataContext> options, AuthenticationService authenticationService)
        {
            _context = new DataContext(options);
            _authenticationService = authenticationService;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> PostTimetable([FromBody] TimetableDb timetable)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var newTimetable = new TimetableDb
            {
                Id = timetable.Id,
                DoctorId = timetable.DoctorId,
                HospitalId = timetable.HospitalId,
                From = timetable.From,
                To = timetable.To,
                Room = timetable.Room
            };

            var hospital = await _authenticationService.GetHospitalAsync(timetable.HospitalId, token);
            if (hospital == timetable.HospitalId)
            {
                var hospitalRoom = await _authenticationService.GetRoomAsync(timetable.HospitalId, token);
                if (hospitalRoom != null && hospitalRoom.Contains($"{timetable.Room}"))
                {
                    var doctor = await _authenticationService.GetDoctorAsync(timetable.DoctorId, token);
                    if (doctor == timetable.DoctorId)
                    {
                        if (timetable.From.Minute % 30 == 0 && timetable.To.Minute % 30 == 0 && timetable.From.Second == 0 && timetable.To.Second == 0)
                        {
                            if (timetable.To > timetable.From)
                            {
                                if ((timetable.To - timetable.From).TotalMinutes < 720)
                                {

                                    _context.Add(newTimetable);
                                    _context.SaveChanges();

                                    return Ok(newTimetable);
                                }

                                return NotFound("Разница между {to} и {from} не должна превышать 12 часов");
                            }

                            return NotFound("{to} должен быть больше {from}");
                        }

                        return NotFound("Оба должны делиться на 30 и иметь секунды равные 0");
                    }

                    return NotFound("Нет такого доктора");
                }

                return NotFound("Нет такой кабинеты");
            }

            return NotFound("Нет такого больницы");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTimetable([FromBody] TimetableDb timetable, [FromRoute] int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetablePut = await _context.Timetables.FirstOrDefaultAsync(u => u.Id == id);
            if (timetablePut == null)
            {
                return NotFound();
            }

            timetablePut.DoctorId = timetable.DoctorId;
            timetablePut.HospitalId = timetable.HospitalId;
            timetablePut.From = timetable.From;
            timetablePut.To = timetable.To;
            timetablePut.Room = timetable.Room;

            var hospital = await _authenticationService.GetHospitalAsync(timetable.HospitalId, token);
            if (hospital == timetable.HospitalId)
            {
                var hospitalRoom = await _authenticationService.GetRoomAsync(timetable.HospitalId, token);
                if (hospitalRoom != null && hospitalRoom.Contains($"{timetable.Room}"))
                {
                    var doctor = await _authenticationService.GetDoctorAsync(timetable.DoctorId, token);
                    if (doctor == timetable.DoctorId)
                    {
                        if (timetable.From.Minute % 30 == 0 && timetable.To.Minute % 30 == 0 && timetable.From.Second == 0 && timetable.To.Second == 0)
                        {
                            if (timetable.To > timetable.From)
                            {
                                if ((timetable.To - timetable.From).TotalMinutes < 720)
                                {

                                    _context.Update(timetablePut);
                                    _context.SaveChanges();

                                    return Ok(timetablePut);
                                }

                                return NotFound("Разница между {to} и {from} не должна превышать 12 часов");
                            }

                            return NotFound("{to} должен быть больше {from}");
                        }

                        return NotFound("Оба должны делиться на 30 и иметь секунды равные 0");
                    }

                    return NotFound("Нет такого доктора");
                }

                return NotFound("Нет такой кабинеты");
            }

            return NotFound("Нет такого больницы");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTimetable([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetablesToDelete = await _context.Timetables.FirstOrDefaultAsync(u => u.Id == id);
            if (timetablesToDelete == null)
            {
                return NotFound();
            }

            _context.Remove(timetablesToDelete);
            await _context.SaveChangesAsync();

            return Ok(timetablesToDelete);
        }

        [HttpDelete("Doctor/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTimetableDoctor([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetablesToDelete = await _context.Timetables.Where(u => u.DoctorId == id).ToListAsync();
            if (timetablesToDelete.Count == 0)
            {
                return NotFound();
            }

            _context.Timetables.RemoveRange(timetablesToDelete);
            await _context.SaveChangesAsync();

            return Ok(timetablesToDelete);
        }

        [HttpDelete("Hospital/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTimetableHospital([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetablesToDelete = await _context.Timetables.Where(u => u.HospitalId == id).ToListAsync();
            if (timetablesToDelete.Count == 0)
            {
                return NotFound();
            }

            _context.Timetables.RemoveRange(timetablesToDelete);
            await _context.SaveChangesAsync();

            return Ok(timetablesToDelete);
        }

        [HttpGet("Hospital/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTimetableHospital([FromRoute] int id, [FromQuery] string? from, [FromQuery] string? to)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetables = await _context.Timetables.Where(t => t.HospitalId == id).ToListAsync();

            DateTimeOffset? fromDate = string.IsNullOrEmpty(from) ? (DateTimeOffset?)null : DateTimeOffset.Parse(from).ToOffset(TimeSpan.FromHours(3));
            DateTimeOffset? toDate = string.IsNullOrEmpty(to) ? (DateTimeOffset?)null : DateTimeOffset.Parse(to).ToOffset(TimeSpan.FromHours(3));

            var matchingTimetables = timetables.AsQueryable();

            if (fromDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.From >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.To <= toDate.Value);
            }

            var result = matchingTimetables.ToList();

            if (!result.Any())
            {
                return NotFound("Нет таких записей");
            }

            return Ok(result);
        }

        [HttpGet("Doctor/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTimetableDoctor([FromRoute] int id, [FromQuery] string? from, [FromQuery] string? to)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager"))
            {
                return Unauthorized();
            }

            var timetables = await _context.Timetables.Where(u => u.DoctorId == id).ToListAsync();

            DateTimeOffset? fromDate = string.IsNullOrEmpty(from) ? (DateTimeOffset?)null : DateTimeOffset.Parse(from).ToOffset(TimeSpan.FromHours(3));
            DateTimeOffset? toDate = string.IsNullOrEmpty(to) ? (DateTimeOffset?)null : DateTimeOffset.Parse(to).ToOffset(TimeSpan.FromHours(3));

            var matchingTimetables = timetables.AsQueryable();

            if (fromDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.From >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.To <= toDate.Value);
            }

            var result = matchingTimetables.ToList();

            if (!result.Any())
            {
                return NotFound("Нет таких записей");
            }

            return Ok(result);
        }

        [HttpGet("Hospital/{id}/Room/{room}")]
        [Authorize]
        public async Task<IActionResult> GetTimetableHospitalAndRoom([FromRoute] int id, [FromRoute] string room, [FromQuery] string? from, [FromQuery] string? to)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (!role.Contains("Admin") && !role.Contains("Manager") && !role.Contains("Doctor"))
            {
                return Unauthorized();
            }

            var timetables = await _context.Timetables.Where(u => u.HospitalId == id && u.Room == room).ToListAsync();

            DateTimeOffset? fromDate = string.IsNullOrEmpty(from) ? (DateTimeOffset?)null : DateTimeOffset.Parse(from).ToOffset(TimeSpan.FromHours(3));
            DateTimeOffset? toDate = string.IsNullOrEmpty(to) ? (DateTimeOffset?)null : DateTimeOffset.Parse(to).ToOffset(TimeSpan.FromHours(3));

            var matchingTimetables = timetables.AsQueryable();

            if (fromDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.From >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                matchingTimetables = matchingTimetables.Where(t => t.To <= toDate.Value);
            }

            var result = matchingTimetables.ToList();

            if (!result.Any())
            {
                return NotFound("Нет таких записей");
            }

            return Ok(result);
        }

        [HttpGet("{id}/Appointments")]
        [Authorize]
        public async Task<IActionResult> GetTimetableAppointments([FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var timetable = await _context.Timetables.FirstOrDefaultAsync(u => u.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            var availableSlots = GetAvailableSlots(timetable.From, timetable.To);
            return Ok(availableSlots);
        }

        [HttpPost("{id}/Appointments")]
        [Authorize]
        public async Task<IActionResult> PostTimetableAppointments([FromBody] AppointmentDb appointment, [FromRoute] int id)
        {
            var role = await _authenticationService.GetValidateAsync(HttpContext);
            if (role == null || role == "NoUser")
            {
                return Unauthorized();
            }

            var timetable = await _context.Timetables.FirstOrDefaultAsync(u => u.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            var json = JObject.Parse(role);
            var ids = json["id"].Value<int>();

            var newavailableSlots = new AppointmentDb
            {
                USerId = ids,
                TimetableId = id,
                Time = appointment.Time
            };

            var availableSlots = GetAvailableSlots(timetable.From, timetable.To);

            if (availableSlots.Contains(appointment.Time))
            {
                _context.Add(newavailableSlots);
                _context.SaveChanges();

                return Ok(newavailableSlots);
            }

            return NotFound("Нет такого расписания");
        }

        private IEnumerable<DateTime> GetAvailableSlots(DateTime from, DateTime to)
        {
            var slots = new List<DateTime>();
            var currentTime = from;

            while (currentTime < to)
            {
                slots.Add(currentTime);
                currentTime = currentTime.AddMinutes(30);
            }

            return slots;
        }
    }
}
