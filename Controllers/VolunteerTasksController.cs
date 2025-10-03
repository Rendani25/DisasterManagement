using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class VolunteerTasksController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VolunteerTasksController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.VolunteerTasks.Include(t => t.VolunteerProfile).ThenInclude(v => v.User).ToListAsync());

        public IActionResult Create()
        {
            ViewBag.Volunteers = _db.VolunteerProfiles
                    .Include(v => v.User)
                    .ToList(); return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(VolunteerTask task)
        {
            if (ModelState.IsValid)
            {
                _db.VolunteerTasks.Add(task);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Volunteers = _db.VolunteerProfiles
       .Include(v => v.User)
       .ToList();

            return View(task);
        }

        public async Task<IActionResult> Details(int id) => View(await _db.VolunteerTasks.Include(t => t.VolunteerProfile).ThenInclude(v => v.User).FirstOrDefaultAsync(t => t.Id == id));

        public async Task<IActionResult> Delete(int id) => View(await _db.VolunteerTasks.FindAsync(id));

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ent = await _db.VolunteerTasks.FindAsync(id);
            if (ent != null) { _db.VolunteerTasks.Remove(ent); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
