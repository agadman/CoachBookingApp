using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoachBookingApp.Models;
using CoachBookingApp.Data;

namespace CoachBookingApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
     public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var coaches = _context.Coaches.ToList(); 
        return View(coaches);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
