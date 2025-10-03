using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.VolunteerProfiles.Include(v => v.Tasks).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VolunteerProfile volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volunteer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }
    }
}
