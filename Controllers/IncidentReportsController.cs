using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class IncidentReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public IncidentReportsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // List all incidents
        public async Task<IActionResult> Index()
        {
            var incidents = await _db.IncidentReports
                                     .Include(i => i.ReportedBy)
                                     .OrderByDescending(i => i.ReportedOn)
                                     .ToListAsync();
            return View(incidents);
        }

        // Show Create form
        public IActionResult Create()
        {
            ViewBag.IncidentTypes = new SelectList(Enum.GetValues(typeof(DisasterType)));
            return View();
        }

        // Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncidentReport model)
        {
            // Get current user info FIRST
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.Identity.Name;

            Console.WriteLine($"User ID: {currentUserId}");
            Console.WriteLine($"User Name: {currentUserName}");

            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["Error"] = "You must be logged in to report an incident.";
                return RedirectToAction("Login", "Account");
            }

            // MANUALLY validate only the fields that come from the form
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Title))
                validationErrors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(model.Description))
                validationErrors.Add("Description is required.");

            if (model.Type == 0 && !Enum.IsDefined(typeof(DisasterType), model.Type)) // Check if Type is valid
                validationErrors.Add("Incident Type is required.");

            // If there are validation errors, return to form
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                ViewBag.IncidentTypes = new SelectList(Enum.GetValues(typeof(DisasterType)));
                return View(model);
            }

            try
            {
                // Create new incident with ALL required fields
                var incidentReport = new IncidentReport
                {
                    Title = model.Title?.Trim(),
                    Description = model.Description?.Trim(),
                    Type = model.Type,
                    Address = model.Address?.Trim(),
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Status = model.Status ?? "Open",
                    // These are the critical fields that were causing the error:
                    ReportedById = currentUserId, // This is now ALWAYS set
                    ReportedOn = DateTime.Now
                };

                // Handle file upload
                if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.AttachmentFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AttachmentFile.CopyToAsync(stream);
                    }

                    incidentReport.AttachmentPath = "/uploads/" + uniqueFileName;
                }

                _db.Add(incidentReport);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Incident report submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while saving the incident. Please try again.");
                ViewBag.IncidentTypes = new SelectList(Enum.GetValues(typeof(DisasterType)));
                return View(model);
            }
        }

        // Show details
        public async Task<IActionResult> Details(int id)
        {
            var incident = await _db.IncidentReports
                                    .Include(i => i.ReportedBy)
                                    .FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null) return NotFound();
            return View(incident);
        }

        // Delete incident
        public async Task<IActionResult> Delete(int id)
        {
            var incident = await _db.IncidentReports.FindAsync(id);
            if (incident == null) return NotFound();
            return View(incident);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ent = await _db.IncidentReports.FindAsync(id);
            if (ent != null)
            {
                // Optionally delete file
                if (!string.IsNullOrEmpty(ent.AttachmentPath))
                {
                    var filePath = Path.Combine(_env.WebRootPath, ent.AttachmentPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _db.IncidentReports.Remove(ent);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}