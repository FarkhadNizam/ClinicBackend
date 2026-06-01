using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(AppDbContext context, ILogger<DoctorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========== Создание врача ==========
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DTO.CreateDoctorDTO doctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _context.Doctors.AnyAsync(d =>
                    d.FirstName == doctorDto.FirstName &&
                    d.LastName == doctorDto.LastName))
                {
                    return Conflict("Врач с таким именем и фамилией уже существует");
                }

                var doctor = new Models.Doctor
                {
                    Id = Guid.NewGuid(),
                    FirstName = doctorDto.FirstName,
                    LastName = doctorDto.LastName,
                    SpecialtyId = doctorDto.SpecialtyId,
                    // ExperienceYears, PlannedWeeklyHours и т.д. — добавь позже
                };

                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании врача");
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }

        // ========== Получение одного врача ==========
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(Guid id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Specialty)           // ← полезно
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            return Ok(MapToDoctorDto(doctor));
        }

        // ========== Список всех врачей ==========
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Specialty)
                .Select(d => MapToDoctorDto(d))
                .ToListAsync();

            return Ok(doctors);
        }

        // ========== Для текущего врача (для роли Doctor) ==========
        [HttpGet("me")]
        public async Task<ActionResult<DoctorDto>> GetCurrentDoctor()
        {
            // В будущем лучше брать из JWT, а не хардкодить!
            var doctorId = Guid.Parse("71b501ff-49ae-4601-8c44-c77fcffb047a");

            var doctor = await _context.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null) return NotFound();

            return Ok(MapToDoctorDto(doctor));
        }

        // ========== Расписание врача ==========
        [HttpGet("available-dates/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableDates(Guid doctorId)
        {
            var dates = await _context.ScheduleSlots
                .Where(s => s.DoctorId == doctorId && s.IsAvailable)
                .Select(s => s.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(dates);
        }

        [HttpGet("available-times/{doctorId}/{date}")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableTimes(Guid doctorId, DateTime date)
        {
            var times = await _context.ScheduleSlots
                .Where(s => s.DoctorId == doctorId &&
                           s.Date.Date == date.Date &&
                           s.IsAvailable)
                .Select(s => s.TimeFrom)
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(times);
        }

        // ========== Приёмы текущего врача ==========
        [HttpGet("appointments")]
        public async Task<ActionResult<List<AppointmentDTO>>> GetDoctorAppointments()
        {
            var doctorId = Guid.Parse("71b501ff-49ae-4601-8c44-c77fcffb047a"); // TODO: брать из User.Claims

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .Include(a => a.ScheduleSlot)
                .OrderBy(a => a.ScheduleSlot.Date)
                .ThenBy(a => a.ScheduleSlot.TimeFrom)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    //PatientName = $"{a.Patient.LastName} {a.Patient.FirstName}",
                    Date = a.ScheduleSlot.Date,
                    Time = a.ScheduleSlot.TimeFrom,
                    //Status = a.status
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // ========== Маппинг (чтобы не дублировать) ==========
        private static DoctorDto MapToDoctorDto(Models.Doctor doctor)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                SpecialtyId = doctor.SpecialtyId,
                SpecialtyName = doctor.Specialty?.Name
            };
        }
    }    
}