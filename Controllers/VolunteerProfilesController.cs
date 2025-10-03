using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class VolunteerProfilesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VolunteerProfilesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.VolunteerProfiles.Include(v => v.User).ToListAsync());

        public IActionResult Create()
        {
            // Load the User dropdown before showing the form
            ViewBag.UserId = new SelectList(_db.Users, "Id", "UserName");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VolunteerProfile volunteer)
        {
            if (!ModelState.IsValid) return View(volunteer);
            _db.VolunteerProfiles.Add(volunteer);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

           ;
        }

        public async Task<IActionResult> Details(int id) => View(await _db.VolunteerProfiles.Include(v => v.User).Include(v => v.Tasks).FirstOrDefaultAsync(v => v.Id == id));

        public async Task<IActionResult> Delete(int id) => View(await _db.VolunteerProfiles.FindAsync(id));

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ent = await _db.VolunteerProfiles.FindAsync(id);
            if (ent != null) { _db.VolunteerProfiles.Remove(ent); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
