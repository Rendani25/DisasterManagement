using DisasterManagement.data;
using DisasterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DisasterManagement.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DonationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Donation
        public async Task<IActionResult> Index()
        {
            var donations = await _db.Donations
                .Include(d => d.Donor)
                .Include(d => d.Items)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();

            return View(donations);
        }

        // GET: Donation/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation model)
        {
            // Get current user ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["Error"] = "You must be logged in to make a donation.";
                return RedirectToAction("Login", "Account");
            }

            // Manual validation for form fields only
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Title))
                validationErrors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(model.Description))
                validationErrors.Add("Description is required.");

            // Check if there are any valid items
            if (model.Items == null || !model.Items.Any() || model.Items.All(i => string.IsNullOrWhiteSpace(i.ItemName)))
                validationErrors.Add("At least one donation item is required.");

            // If validation errors, return to form
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }

            try
            {
                // Create NEW donation object with all required fields
                var donation = new Donation
                {
                    Title = model.Title.Trim(),
                    Description = model.Description.Trim(),
                    Status = "Pending", // Always start as Pending
                    CreatedOn = DateTime.UtcNow,
                    DonorId = currentUserId, // This is the key - set the required field
                    Items = new List<DonationItem>()
                };

                // Add items (filter out empty ones)
                foreach (var itemModel in model.Items)
                {
                    if (!string.IsNullOrWhiteSpace(itemModel.ItemName))
                    {
                        donation.Items.Add(new DonationItem
                        {
                            ItemName = itemModel.ItemName.Trim(),
                            Quantity = itemModel.Quantity > 0 ? itemModel.Quantity : 1
                        });
                    }
                }

                _db.Donations.Add(donation);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Donation submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the donation. Please try again.");
                Console.WriteLine($"Error: {ex.Message}");
                return View(model);
            }
        }

        // GET: Donations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var donation = await _db.Donations
                .Include(d => d.Donor)
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.DonationId == id);

            if (donation == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (donation.DonorId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You can only delete your own donations.";
                return RedirectToAction(nameof(Index));
            }

            return View(donation);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donation = await _db.Donations
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.DonationId == id);

            if (donation != null)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (donation.DonorId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You can only delete your own donations.";
                    return RedirectToAction(nameof(Index));
                }

                _db.DonationItems.RemoveRange(donation.Items);
                _db.Donations.Remove(donation);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Donation deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}