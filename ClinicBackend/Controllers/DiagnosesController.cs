using ClinicBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiagnosesController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string search = "")
        {
            var diagnoses =
                await _context.Diagnoses
                    .Where(d =>
                        d.Name.Contains(search) ||
                        d.MkbCode.Contains(search))
                    .OrderBy(d => d.MkbCode)
                    .Take(20)
                    .Select(d => new
                    {
                        d.DiagnosisId,
                        d.MkbCode,
                        d.Name,
                        d.Category
                    })
                    .ToListAsync();

            return Ok(diagnoses);
        }
    }
}
