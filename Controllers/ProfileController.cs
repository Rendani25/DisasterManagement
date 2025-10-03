using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager) { _userManager = userManager; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = new ProfileViewModel { Email = user.Email ?? "", FullName = user.FullName, Address = user.Address, IsDonor = user.IsDonor, IsVolunteer = user.IsVolunteer };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.IsDonor = model.IsDonor;
            user.IsVolunteer = model.IsVolunteer;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError("", e.Description); return View(model); }
            ViewBag.Message = "Profile updated";
            return View(model);
        }
    }
}
