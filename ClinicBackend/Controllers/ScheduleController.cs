using ClinicBackend.Data;
using ClinicBackend.Models;
using ClinicBackend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBackend.DTO;

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

                // Валидация времени
                if (!TimeSpan.TryParse(request.StartTime, out var startTime) ||
                    !TimeSpan.TryParse(request.EndTime, out var endTime))
                {
                    return BadRequest("Некорректный формат времени");
                }

                if (startTime >= endTime)
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
                    startTime,
                    endTime,
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
            string date,
            TimeSpan startTime,
            TimeSpan endTime,
            int durationMinutes,
            string doctorId)
        {
            var slots = new List<ScheduleSlot>();
            var currentTime = startTime;
            var duration = TimeSpan.FromMinutes(durationMinutes);

            while (currentTime + duration <= endTime)
            {
                slots.Add(new ScheduleSlot
                {
                    Id = Guid.NewGuid().ToString(),
                    DoctorId = doctorId,
                    Date = date,
                    TimeFrom = currentTime.ToString(@"hh\:mm"),
                    TimeTo = (currentTime + duration).ToString(@"hh\:mm"),
                    IsAvailable = true
                });

                currentTime = currentTime + duration;
            }

            return slots;
        }
    }
}