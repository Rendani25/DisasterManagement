using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Main view - Manage Volunteer Tasks
        public async Task<IActionResult> Manage()
        {
            var volunteerTasks = await _context.VolunteerTasks
                .Include(v => v.AssignedVolunteer)  // Include the ApplicationUser
                .Include(v => v.VolunteerProfile)   // Include the VolunteerProfile
                    .ThenInclude(vp => vp.User)     // Include User from VolunteerProfile
                .OrderByDescending(v => v.StartTime)
                .ToListAsync();

            return View(volunteerTasks);
        }

        // Create Task - will use Manage view with modal or separate form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(VolunteerTask task)
        {
            if (ModelState.IsValid)
            {
                _context.VolunteerTasks.Add(task);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Volunteer task created successfully!";
            }
            else
            {
                TempData["Error"] = "Please fill in all required fields.";
            }

            return RedirectToAction(nameof(Manage));
        }

        // Update Task Status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var task = await _context.VolunteerTasks.FindAsync(id);
            if (task == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Manage));
            }

            task.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Task status updated to {status}";
            return RedirectToAction(nameof(Manage));
        }

        // Delete Task
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.VolunteerTasks.FindAsync(id);
            if (task != null)
            {
                _context.VolunteerTasks.Remove(task);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Volunteer task deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Task not found.";
            }

            return RedirectToAction(nameof(Manage));
        }

        // Assign Task to Volunteer
        [HttpPost]
        public async Task<IActionResult> AssignTask(int taskId, string volunteerId)
        {
            var task = await _context.VolunteerTasks.FindAsync(taskId);
            if (task == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Manage));
            }

            task.AssignedVolunteerId = volunteerId;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Task assigned successfully!";
            return RedirectToAction(nameof(Manage));
        }

        private bool VolunteerTaskExists(int id)
        {
            return _context.VolunteerTasks.Any(e => e.Id == id);
        }
    }
}