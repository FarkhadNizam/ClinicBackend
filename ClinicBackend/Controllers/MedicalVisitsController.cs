// Controllers/MedicalVisitsController.cs
using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MedicalVisitsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicalVisitsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{appointmentId}/visit")]
    public async Task<IActionResult> GetVisit(
    Guid appointmentId)
    {
        var appointment =
            await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.MedicalVisit)
                    .ThenInclude(v => v.Diagnosis)
                .FirstOrDefaultAsync(
                    a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound();

        var currentVisitId = appointment.MedicalVisit?.Id;

        var history =
            await _context.MedicalVisits
                .Include(v => v.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(v => v.Diagnosis)
                .Where(v =>
                    v.Appointment.PatientId == appointment.PatientId &&
                    v.Id != currentVisitId)
                .OrderByDescending(v => v.Date)
                .Take(20)
                .Select(v => new MedicalVisitHistoryDto
                {
                    Id = v.Id,
                    Date = v.Date,
                    Complaints = v.Complaints,
                    Treatment = v.Treatment,
                    DiagnosisName = v.Diagnosis.Name,
                    DoctorName = v.Appointment.Doctor.LastName
                })
                .ToListAsync();

        var result =
            new AppointmentVisitDto
            {
                AppointmentId = appointment.Id,

                Patient = new PatientDto
                {
                    Id = appointment.Patient.Id,
                    FirstName = appointment.Patient.FirstName,
                    LastName = appointment.Patient.LastName,
                    BirthDate = appointment.Patient.BirthDate,
                    Phone = appointment.Patient.Phone,
                    InsuranceNumber =
                        appointment.Patient.InsuranceNumber
                },

                Visit =
                    appointment.MedicalVisit == null
                    ? null
                    : new MedicalVisitDto
                    {
                        Id =
                            appointment.MedicalVisit.Id,

                        Date =
                            appointment.MedicalVisit.Date,

                        Complaints =
                            appointment.MedicalVisit.Complaints,

                        Treatment =
                            appointment.MedicalVisit.Treatment,

                        DiagnosisId =
                            appointment.MedicalVisit.DiagnosisId,

                        DiagnosisName =
                            appointment.MedicalVisit
                                .Diagnosis?.Name,

                        DiagnosisCode =
                            appointment.MedicalVisit
                                .Diagnosis?.MkbCode
                    },

                History = history
            };

        return Ok(result);
    }

    [HttpPost("{appointmentId}/visit")]
    public async Task<IActionResult> CreateVisit(Guid appointmentId,
    CreateMedicalVisitDto dto)
    {
        var appointment =
            await _context.Appointments
                .FirstOrDefaultAsync(
                    a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound();

        if (appointment.MedicalVisit != null)
            return BadRequest(
                "Прием уже оформлен");

        var visit = new MedicalVisit
        {
            Id = Guid.NewGuid(),

            AppointmentId = appointmentId,

            Complaints = dto.Complaints,

            DiagnosisId = dto.DiagnosisId,

            Treatment = dto.Treatment,

            Date = DateTime.UtcNow
        };

        appointment.status =
            Appointment.Status.Completed;

        _context.MedicalVisits.Add(visit);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{appointmentId}/visit")]
    public async Task<ActionResult> UpdateVisit(string id, [FromBody] UpdateMedicalVisitDto visitDto)
    {
        var existing = await _context.MedicalVisits.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Complaints = visitDto.Complaints;
        existing.DiagnosisId = visitDto.DiagnosisId;
        existing.Treatment = visitDto.Treatment;        

        await _context.SaveChangesAsync();
        return Ok();
    }    
}