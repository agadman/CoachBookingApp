using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoachBookingApp.Data;
using CoachBookingApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoachBookingApp.Controllers
{
    [Authorize(Roles = "Admin,Coach")]
    public class TimeSlotController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TimeSlot
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Admin"))
            {
                var allSlots = await _context.Timeslots
                    .Include(t => t.Coach)
                    .ToListAsync();

                return View(allSlots);
            }

            if (User.IsInRole("Coach"))
            {
                var coachSlots = await _context.Timeslots
                    .Include(t => t.Coach)
                    .Where(t => t.Coach != null && t.Coach.UserId == userId)
                    .ToListAsync();

                return View(coachSlots);
            }

            return Forbid();
        }

        // GET: TimeSlot/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.Timeslots
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // GET: TimeSlot/Create
        [Authorize(Roles = "Admin,Coach")]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Name");
            }

            return View();
        }

        // POST: TimeSlot/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime,CoachId")] TimeSlot timeSlot)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Coach"))
{
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (coach == null)
                    return Forbid();

                timeSlot.CoachId = coach.Id;
            }

            if (ModelState.IsValid)
{
                // Konverterar från svensk tid till UTC innan det sparas i databasen
                var swedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                timeSlot.StartTime = TimeZoneInfo.ConvertTimeToUtc(timeSlot.StartTime, swedenTimeZone);
                timeSlot.EndTime = TimeZoneInfo.ConvertTimeToUtc(timeSlot.EndTime, swedenTimeZone);

                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(timeSlot);
        }

        // GET: TimeSlot/Edit/5
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var timeSlot = await _context.Timeslots
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (timeSlot == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Coach") && (timeSlot.Coach == null || timeSlot.Coach.UserId != userId))
                return Forbid();

            return View(timeSlot);
        }

        // POST: TimeSlot/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,CoachId")] TimeSlot timeSlot)
        {
            if (id != timeSlot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Konverterar från svensk tid till UTC innan uppdatering till databasen
                    var swedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                    timeSlot.StartTime = TimeZoneInfo.ConvertTimeToUtc(timeSlot.StartTime, swedenTimeZone);
                    timeSlot.EndTime = TimeZoneInfo.ConvertTimeToUtc(timeSlot.EndTime, swedenTimeZone);

                    _context.Update(timeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeSlotExists(timeSlot.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Name", timeSlot.CoachId);
            return View(timeSlot);
        }

       // GET: TimeSlot/Delete/5
       [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var timeSlot = await _context.Timeslots
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (timeSlot == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Coach får bara sina egna slots
            if (User.IsInRole("Coach") && (timeSlot.Coach == null || timeSlot.Coach.UserId != userId))
                return Forbid();

            return View(timeSlot);
        }

        // POST: TimeSlot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeSlot = await _context.Timeslots
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (timeSlot == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Coach") && (timeSlot.Coach == null || timeSlot.Coach.UserId != userId))
                return Forbid();

            _context.Timeslots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TimeSlotExists(int id)
        {
            return _context.Timeslots.Any(e => e.Id == id);
        }
    }
}
 