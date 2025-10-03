using Microsoft.AspNetCore.Mvc;

namespace DisasterManagement.Controllers
{
    public class LandingController : Controller
    {
        // Public-facing entry page
        [HttpGet]
        public IActionResult Welcome()
        {
            return View(); // Views/Landing/Index.cshtml
        }
    }
}
