using ClinicBackend.Data;
using ClinicBackend.Models;
using ClinicBackend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PatientController> _logger;

        public PatientController(AppDbContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDTO patientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var patient = new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    BirthDate = patientDto.BirthDate,
                    Phone = patientDto.Phone,
                    InsuranceNumber = patientDto.InsuranceNumber
                };

                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пациента");
                return StatusCode(500, "Произошла ошибка при создании пациента");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return patient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            return await _context.Patients
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    BirthDate = p.BirthDate
                })
                .ToListAsync();
        }
    }

    public class PatientDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        public string? Phone { get; set; }

        public string? InsuranceNumber { get; set; }
    }

    public class CreatePatientDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        public string? Phone { get; set; }

        public string? InsuranceNumber { get; set; }
    }
}