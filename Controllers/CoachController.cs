using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachBookingApp.Controllers
{
    [Authorize(Roles = "Coach")]
    public class CoachController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}