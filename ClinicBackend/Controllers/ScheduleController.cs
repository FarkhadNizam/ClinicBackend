using ClinicBackend.Data;
using ClinicBackend.Models;
using ClinicBackend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(AppDbContext context, ILogger<ScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSchedule([FromBody] GenerateScheduleDTO request)
        {
            try
            {
                // Проверка существования врача
                var doctor = await _context.Doctors.FindAsync(request.DoctorId);
                if (doctor == null)
                {
                    return NotFound("Врач не найден");
                }


                if (request.StartTime >= request.EndTime)
                {
                    return BadRequest("Время окончания должно быть позже времени начала");
                }

                if (request.DurationMinutes <= 0)
                {
                    return BadRequest("Длительность приема должна быть положительной");
                }

                // Проверка на существующие слоты в эту дату
                var existingSlots = await _context.ScheduleSlots
                    .Where(s => s.DoctorId == request.DoctorId && s.Date == request.Date)
                    .ToListAsync();

                if (existingSlots.Any())
                {
                    return Conflict("Расписание для этого врача на выбранную дату уже существует");
                }

                // Генерация слотов
                var generatedSlots = GenerateTimeSlots(
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    request.DurationMinutes,
                    request.DoctorId);

                await _context.ScheduleSlots.AddRangeAsync(generatedSlots);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    GeneratedCount = generatedSlots.Count,
                    DoctorName = $"{doctor.LastName} {doctor.FirstName[0]}.",
                    Date = request.Date
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации расписания");
                return StatusCode(500, "Произошла ошибка при генерации расписания");
            }
        }

        private List<ScheduleSlot> GenerateTimeSlots(
            DateTime date,
            DateTime startTime,
            DateTime endTime,
            int durationMinutes,
            Guid doctorId)
        {
            var slots = new List<ScheduleSlot>();
            var currentTime = startTime;
            var duration = TimeSpan.FromMinutes(durationMinutes);

            while (currentTime + duration <= endTime)
            {
                slots.Add(new ScheduleSlot
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    Date = date,
                    TimeFrom = currentTime,
                    TimeTo = currentTime + duration,
                    IsAvailable = true
                });

                currentTime = currentTime + duration;
            }

            return slots;
        }
    }
}