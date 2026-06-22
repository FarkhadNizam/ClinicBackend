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
    public async Task<IActionResult> GetVisit(Guid appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.MedicalVisit)
                .ThenInclude(v => v!.Diagnosis)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound();

        // ========== История визитов ==========
        var historyQuery = _context.MedicalVisits
            .Include(v => v.Appointment)
                .ThenInclude(a => a.Doctor)
            .Include(v => v.Diagnosis)
            .Where(v => v.Appointment.PatientId == appointment.PatientId);

        // Исключаем текущий визит, если он уже существует
        if (appointment.MedicalVisit != null)
        {
            historyQuery = historyQuery.Where(v => v.Id != appointment.MedicalVisit.Id);
        }

        var history = await historyQuery
            .OrderByDescending(v => v.Date)
            .Take(20)
            .Select(v => new MedicalVisitHistoryDto
            {
                Id = v.Id,
                Date = v.Date,
                Complaints = v.Complaints,
                Treatment = v.Treatment,
                DiagnosisName = v.Diagnosis != null ? v.Diagnosis.Name : null,
                DoctorName = $"{v.Appointment.Doctor.LastName} {v.Appointment.Doctor.FirstName}"
            })
            .ToListAsync();

        // ========== Формируем ответ ==========
        var result = new AppointmentVisitDto
        {
            AppointmentId = appointment.Id,

            Patient = new PatientDto
            {
                Id = appointment.Patient.Id,
                FirstName = appointment.Patient.FirstName,
                LastName = appointment.Patient.LastName,
                BirthDate = appointment.Patient.BirthDate,
                Phone = appointment.Patient.Phone,
                InsuranceNumber = appointment.Patient.InsuranceNumber
            },

            Visit = appointment.MedicalVisit == null ? null : new MedicalVisitDto
            {
                Id = appointment.MedicalVisit.Id,
                Date = appointment.MedicalVisit.Date,
                Complaints = appointment.MedicalVisit.Complaints,
                Treatment = appointment.MedicalVisit.Treatment,
                DiagnosisId = appointment.MedicalVisit.DiagnosisId,
                DiagnosisName = appointment.MedicalVisit.Diagnosis?.Name,
                DiagnosisCode = appointment.MedicalVisit.Diagnosis?.MkbCode
            },

            History = history
        };

        return Ok(result);
    }

    [HttpPost("{appointmentId}/visit")]
    public async Task<IActionResult> CreateVisit(Guid appointmentId, [FromBody] CreateMedicalVisitDto dto)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null) return NotFound();
        if (appointment.MedicalVisit != null) return BadRequest("Прием уже оформлен");

        var visit = new MedicalVisit
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            Complaints = dto.Complaints,
            DiagnosisId = dto.DiagnosisId,
            Treatment = dto.Treatment,
            Date = DateTime.UtcNow
        };

        appointment.status = Appointment.Status.Completed;

        _context.MedicalVisits.Add(visit);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{appointmentId}/visit")]
    public async Task<IActionResult> UpdateVisit(Guid appointmentId, [FromBody] UpdateMedicalVisitDto dto)
    {
        var visit = await _context.MedicalVisits
            .FirstOrDefaultAsync(v => v.AppointmentId == appointmentId);

        if (visit == null) return NotFound("Прием не найден");

        visit.Complaints = dto.Complaints;
        visit.DiagnosisId = dto.DiagnosisId;
        visit.Treatment = dto.Treatment;
        visit.Date = DateTime.UtcNow; // обновляем дату редактирования

        await _context.SaveChangesAsync();

        return Ok();
    }    
}