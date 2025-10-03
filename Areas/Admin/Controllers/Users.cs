using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get all users first
            var users = await _userManager.Users.ToListAsync();

            var usersWithDetails = new List<dynamic>();

            // Then process each user individually
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isVolunteer = await _context.VolunteerProfiles
                    .AnyAsync(v => v.UserId == user.Id);
                var isDonor = await _context.Donations
                    .AnyAsync(d => d.DonorId == user.Id);

                usersWithDetails.Add(new
                {
                    User = user,
                    Roles = roles,
                    IsVolunteer = isVolunteer,
                    IsDonor = isDonor
                });
            }

            ViewBag.UsersWithDetails = usersWithDetails;
            return View();
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var volunteerTask = await _context.VolunteerProfiles
                .FirstOrDefaultAsync(v => v.UserId == id);
            var donationsCount = await _context.Donations
                .CountAsync(d => d.DonorId == id);
            var incidentsCount = await _context.IncidentReports
                .CountAsync(i => i.ReportedById == id);

            ViewBag.UserRoles = userRoles;
            ViewBag.VolunteerProfile = volunteerTask;
            ViewBag.DonationsCount = donationsCount;
            ViewBag.IncidentsCount = incidentsCount;

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var profile = new ProfileViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                IsVolunteer = await _context.VolunteerProfiles.AnyAsync(v => v.UserId == id),
                IsDonor = await _context.Donations.AnyAsync(d => d.DonorId == id)
            };

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View(profile);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProfileViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update user properties
                    user.FullName = model.FullName;
                    user.Address = model.Address;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "User profile updated successfully!";
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (isInRole)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
                TempData["Success"] = $"Removed {roleName} role from {user.UserName}";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, roleName);
                TempData["Success"] = $"Added {roleName} role to {user.UserName}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deleting themselves
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.UserName} deleted successfully";
            }
            else
            {
                TempData["Error"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(string id)
        {
            return await _userManager.FindByIdAsync(id) != null;
        }
    }
}