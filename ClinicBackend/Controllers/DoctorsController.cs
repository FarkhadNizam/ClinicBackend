// Controllers/DoctorsController.cs
using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }

    // Временно возвращаем фиксированного врача
    [HttpGet("me")]
    public async Task<ActionResult<DoctorDto>> GetCurrentDoctor()
    {
        var doctorId = "71b501ff-49ae-4601-8c44-c77fcffb047a";

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == new Guid(doctorId));
        if (doctor == null) return NotFound();

        return Ok(new DoctorDto
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Specialty = doctor.Specialty
        });
    }

    [HttpGet("appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments()
    {
        var doctorId = "71b501ff-49ae-4601-8c44-c77fcffb047a";

        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == Guid.Parse(doctorId))
            .Include(a => a.Patient)
            .Include(a => a.ScheduleSlot)
            .OrderBy(a => a.ScheduleSlot.Date)
            .ThenBy(a => a.ScheduleSlot.TimeFrom)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientName = $"{a.Patient.LastName} {a.Patient.FirstName}",
                Date = a.ScheduleSlot.Date,
                Time = a.ScheduleSlot.TimeFrom,
                PatientId = a.Patient.Id,
            })
            .ToListAsync();

        return Ok(appointments);
    }
}
// DTOs/AppointmentDto.cs
public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime Date { get; set; }
    public DateTime Time { get; set; }
}

// DTOs/DoctorDto.cs
public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialty { get; set; }
}