using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class IncidentReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncidentReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var incidents = await _context.IncidentReports
                .Include(i => i.ReportedBy)
                .OrderByDescending(i => i.ReportedOn)
                .ToListAsync();

            return View(incidents);
        }
    }
}