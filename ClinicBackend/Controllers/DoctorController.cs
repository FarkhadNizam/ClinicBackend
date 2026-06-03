using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(AppDbContext context, ILogger<DoctorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDTO doctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _context.Doctors.AnyAsync(d =>
                    d.FirstName == doctorDto.FirstName &&
                    d.LastName == doctorDto.LastName))
                {
                    return Conflict("Врач с таким именем уже существует");
                }

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FirstName = doctorDto.FirstName,
                    LastName = doctorDto.LastName,
                    Specialty = doctorDto.Specialty
                    // Avatar не указываем - будет null
                };

                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании врача");
                return StatusCode(500, "Произошла ошибка при создании врача");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            return await _context.Doctors
                .Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Specialty = d.Specialty
                })
                .ToListAsync();
        }

        [HttpGet("available-dates/{doctorId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableDates(Guid doctorId)
        {
            var dates = await _context.ScheduleSlots
                .Where(s => s.DoctorId == doctorId && s.IsAvailable)
                .Select(s => s.Date)
                .Distinct()
                .ToListAsync();

            return Ok(dates);
        }

        [HttpGet("available-times/{doctorId}/{date}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableTimes(
            Guid doctorId,
            DateTime date)
        {
            var times = await _context.ScheduleSlots
                .Where(s => s.DoctorId == doctorId &&
                           s.Date == date &&
                           s.IsAvailable)
                .Select(s => s.TimeFrom)
                .ToListAsync();

            return Ok(times);
        }
    }
}