using ClinicBackend.Data;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/specialties")]
    public class SpecialitiesControler
    {
        private readonly AppDbContext _context;

        public SpecialitiesControler(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Specialty>>>
            GetSpecialties()
        {
            return await _context.Specialties.ToListAsync();
        }
    }
}
