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
using System.Security.Claims;

namespace CoachBookingApp.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings.Include(b => b.TimeSlot);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
{
            var freeSlots = _context.Timeslots
                .Include(t => t.Coach)
                .Include(t => t.Booking) 
                .Where(t => t.Booking == null && t.StartTime > DateTime.Now)
                .OrderBy(t => t.StartTime)
                .ToList();

            ViewData["TimeSlotId"] = freeSlots.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Coach!.Name + " - " + t.StartTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
     [HttpPost]
     [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(Booking booking)
      {
        // Sätt automatiska fält FÖRE validering
        booking.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        booking.Status = "Booked";
        booking.CreatedAt = DateTime.UtcNow;

        var slot = await _context.Timeslots
            .Include(t => t.Booking)
            .FirstOrDefaultAsync(t => t.Id == booking.TimeSlotId);

            if (slot == null || slot.Booking != null)
            {
                ModelState.AddModelError("TimeSlotId", "Denna tid är redan bokad eller finns inte.");
            }

        if (ModelState.IsValid)
        {
            // Fyll i fälten för booking
            booking.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            booking.Status = "Booked";
            booking.CreatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Om fel: fyll dropdownen igen
        ViewData["TimeSlotId"] = _context.Timeslots
            .Include(t => t.Coach)
            .Include(t => t.Booking) // inkludera Booking
            .Where(t => t.Booking == null && t.StartTime > DateTime.Now)
            .OrderBy(t => t.StartTime)
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Coach!.Name + " - " + t.StartTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return View(booking);
        }
        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["TimeSlotId"] = _context.Timeslots
                .Include(t => t.Booking)
                .Where(t =>
                    t.Booking == null || t.Id == booking.TimeSlotId)
                .OrderBy(t => t.StartTime)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    Selected = t.Id == booking.TimeSlotId
                }).ToList();
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerEmail,Status,CreatedAt,TimeSlotId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["TimeSlotId"] = _context.Timeslots
                .Include(t => t.Booking)
                .Where(t =>
                    t.Booking == null || t.Id == booking.TimeSlotId)
                .OrderBy(t => t.StartTime)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    Selected = t.Id == booking.TimeSlotId
                }).ToList();
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);

           if (booking != null)
           {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
           }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
