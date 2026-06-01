using ClinicBackend.Data;
using ClinicBackend.Models;
using ClinicBackend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            AppDbContext context,
            ILogger<AppointmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] AppointmentDTO request)
        {
            try
            {
                // Проверяем существование пациента
                var patient = await _context.Patients.FindAsync(request.PatientId);
                if (patient == null)
                    return NotFound("Пациент не найден");

                // Проверяем существование врача
                var doctor = await _context.Doctors.FindAsync(request.DoctorId);
                if (doctor == null)
                    return NotFound("Врач не найден");

                // Находим доступный слот
                var slot = await _context.ScheduleSlots
                    .FirstOrDefaultAsync(s =>
                        s.DoctorId == request.DoctorId &&
                        s.Date == request.Date &&
                        s.TimeFrom == request.Time &&
                        s.IsAvailable);

                if (slot == null)
                    return BadRequest("Выбранный слот недоступен");

                // Создаем запись
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    ScheduleSlotId = slot.Id,
                    CreatedAt = DateTime.UtcNow
                };

                // Помечаем слот как занятый
                slot.IsAvailable = false;

                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Id = appointment.Id,
                    PatientName = $"{patient.LastName} {patient.FirstName}",
                    DoctorName = $"{doctor.LastName} {doctor.FirstName}",
                    Date = request.Date,
                    Time = request.Time
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании записи");
                return StatusCode(500, "Произошла ошибка при создании записи");
            }
        }

    }       
}