using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Manage()
        {
            var donations = await _context.Donations
                .Include(d => d.Donor)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();

            return View(donations);
        }
    }
}