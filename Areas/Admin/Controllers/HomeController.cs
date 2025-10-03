using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Admin dashboard - show ALL data, not filtered by user
            var model = new HomeViewModel
            {
                TotalIncidents = await _context.IncidentReports.CountAsync(),
                TotalDonations = await _context.Donations.CountAsync(),
                TotalVolunteers = await _context.VolunteerTasks.CountAsync(),

                RecentIncidents = await _context.IncidentReports
                    .OrderByDescending(i => i.ReportedOn)
                    .Take(5)
                    .ToListAsync(),

                RecentDonations = await _context.Donations
                    .Include(d => d.Donor)
                    .OrderByDescending(d => d.CreatedOn)
                    .Take(5)
                    .ToListAsync(),

                RecentTasks = await _context.VolunteerTasks
                    .OrderByDescending(t => t.StartTime)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy() => View();
    }
}