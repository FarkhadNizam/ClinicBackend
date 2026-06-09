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
        public async Task<ActionResult<GenerateScheduleResponse>> GenerateSchedule(
            [FromBody] GenerateScheduleRequest request)
        {
            // Валидация
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.DoctorId == Guid.Empty)
                return BadRequest("Не указан врач");

            if (string.IsNullOrWhiteSpace(request.Date) ||
                string.IsNullOrWhiteSpace(request.StartTime) ||
                string.IsNullOrWhiteSpace(request.EndTime))
                return BadRequest("Дата и время обязательны");

            if (request.DurationMinutes < 15)
                return BadRequest("Минимальная длительность приёма — 15 минут");

            try
            {
                // Проверяем существование врача
                var doctorExists = await _context.Doctors
                    .AnyAsync(d => d.Id == request.DoctorId);

                if (!doctorExists)
                    return NotFound("Врач не найден");

                // Парсим дату
                if (!DateTime.TryParseExact(request.Date, "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    return BadRequest("Неверный формат даты. Ожидается: yyyy-MM-dd");
                }

                // Парсим время
                if (!DateTime.TryParse(request.StartTime, out DateTime startTime) ||
                    !DateTime.TryParse(request.EndTime, out DateTime endTime))
                {
                    return BadRequest("Неверный формат времени. Ожидается: HH:mm");
                }

                if (endTime <= startTime)
                    return BadRequest("Время окончания должно быть позже времени начала");

                int generatedCount = 0;
                var currentTime = startTime;

                // Генерация слотов
                while (currentTime.AddMinutes(request.DurationMinutes) <= endTime)
                {
                    var slot = new ScheduleSlot
                    {
                        Id = Guid.NewGuid(),
                        DoctorId = request.DoctorId,
                        Date = date,
                        TimeFrom = currentTime,
                        TimeTo = currentTime.AddMinutes(request.DurationMinutes),
                        IsAvailable = true
                    };

                    _context.ScheduleSlots.Add(slot);
                    currentTime = currentTime.AddMinutes(request.DurationMinutes);
                    generatedCount++;
                }

                if (generatedCount == 0)
                    return BadRequest("Не удалось создать ни одного слота. Проверьте интервал времени.");

                await _context.SaveChangesAsync();

                return Ok(new GenerateScheduleResponse
                {
                    GeneratedCount = generatedCount,
                    Message = $"Успешно создано {generatedCount} слотов расписания."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка генерации расписания для врача {DoctorId}", request.DoctorId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }        
        }        
    }
}