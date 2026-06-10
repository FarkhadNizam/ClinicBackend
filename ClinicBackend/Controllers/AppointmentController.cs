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
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO request)
        {
            var patient =
                await _context.Patients.FindAsync(
                    request.PatientId);

            if (patient == null)
                return NotFound("Пациент не найден");

            var slot =
                await _context.ScheduleSlots
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(
                        s => s.Id ==
                        request.ScheduleSlotId);

            if (slot == null)
                return NotFound("Слот не найден");

            if (!slot.IsAvailable)
                return BadRequest(
                    "Слот уже занят");

            var duplicate =
                await _context.Appointments
                    .Include(a => a.ScheduleSlot)
                    .AnyAsync(a =>
                        a.PatientId ==
                        request.PatientId &&
                        a.ScheduleSlot.Date ==
                        slot.Date &&
                        a.ScheduleSlot.TimeFrom ==
                        slot.TimeFrom &&
                        a.status !=
                        Appointment.Status.Canceled);

            if (duplicate)
                return BadRequest(
                    "Пациент уже записан");

            var appointment =
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patient.Id,
                    DoctorId = slot.DoctorId,
                    ScheduleSlotId = slot.Id,
                    CreatedAt = DateTime.UtcNow,
                    StatusUpdatedAt = DateTime.UtcNow,
                    status =
                        Appointment.Status.Planned
                };

            slot.IsAvailable = false;

            _context.Appointments.Add(
                appointment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ScheduleSlotId = appointment.ScheduleSlotId,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.status.ToString()
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentListDto>>> GetAppointments()
        {
            var appointments =
                await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.Specialty)
                    .Include(a => a.ScheduleSlot)
                    .OrderBy(a => a.ScheduleSlot.Date)
                    .ThenBy(a => a.ScheduleSlot.TimeFrom)
                    .Select(a => new AppointmentListDto
                    {
                        Id = a.Id,

                        PatientName =
                            a.Patient.LastName + " " +
                            a.Patient.FirstName,

                        DoctorName =
                            a.Doctor.LastName + " " +
                            a.Doctor.FirstName,

                        SpecialtyName =
                            a.Doctor.Specialty.Name,

                        Date =
                            a.ScheduleSlot.Date,

                        TimeFrom =
                            a.ScheduleSlot.TimeFrom,

                        TimeTo =
                            a.ScheduleSlot.TimeTo,

                        Status =
                            a.status,

                    })
                    .ToListAsync();

            return Ok(appointments);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            var appointment =
                await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.status =
                Appointment.Status.Canceled;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }       
}