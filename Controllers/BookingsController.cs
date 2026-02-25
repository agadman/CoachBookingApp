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
        [Authorize(Roles = "Admin,Coach,User")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Admin"))
            {
                var allBookings = await _context.Bookings
                    .Include(b => b.TimeSlot)
                    .ThenInclude(t => t.Coach)
                    .ToListAsync();

                return View(allBookings);
            }

            if (User.IsInRole("User"))
            {
                var myBookings = await _context.Bookings
                    .Include(b => b.TimeSlot)
                    .ThenInclude(t => t.Coach)
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                return View(myBookings);
            }

            if (User.IsInRole("Coach"))
            {
                var coachBookings = await _context.Bookings
                    .Include(b => b.TimeSlot)
                    .ThenInclude(t => t.Coach)
                    .Where(b => b.TimeSlot.Coach.UserId == userId)
                    .ToListAsync();

                return View(coachBookings);
            }

            return Forbid();
        }

        // GET: Bookings/Details/5
        [Authorize(Roles = "Admin,Coach,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .ThenInclude(t => t.Coach)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Admin får se allt
            if (User.IsInRole("Admin"))
                return View(booking);

            // Uuser får bara se sina egna bokningar
            if (User.IsInRole("User") && booking.UserId == userId)
                return View(booking);

            // Coach får bara se bokningar på sina egna timeslots
            if (User.IsInRole("Coach") && 
                booking.TimeSlot?.Coach?.UserId == userId)
                return View(booking);

            return Forbid();
        }

        // GET: Bookings/Create
        [Authorize(Roles = "Admin,User,Coach")]
        public IActionResult Create(int? timeSlotId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var freeSlotsQuery = _context.Timeslots
                .Include(t => t.Coach)
                .Include(t => t.Booking)
                .Where(t => t.Booking == null && t.StartTime > DateTime.Now);

            if (User.IsInRole("Coach"))
            {
                freeSlotsQuery = freeSlotsQuery
                    .Where(t => t.Coach.UserId == userId);
            }

            var freeSlots = freeSlotsQuery
                .OrderBy(t => t.StartTime)
                .ToList();

            ViewData["TimeSlotId"] = freeSlots.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Coach!.Name + " - " + t.StartTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            var booking = new Booking();

            if (User.IsInRole("User"))
            {
                booking.CustomerName = User.Identity?.Name ?? "";
                booking.CustomerEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            }

            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(Booking booking)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var slot = await _context.Timeslots
                .Include(t => t.Coach)
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.Id == booking.TimeSlotId);

            if (slot == null || slot.Booking != null)
            {
                ModelState.AddModelError("TimeSlotId", "Denna tid är redan bokad eller finns inte.");
            }

            // Coach får bara boka sina egna tider
            if (User.IsInRole("Coach") && slot?.Coach?.UserId != userId)
            {
                return Forbid();
            }

            if (User.IsInRole("User"))
            {
                booking.UserId = userId!;
            }

            if (User.IsInRole("Coach"))
            {
                booking.UserId = null;
            }

            booking.Status = "Booked";
            booking.CreatedAt = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // POST Bookings bara för users
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookTimeSlot(int timeSlotId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var userName = User.Identity?.Name ?? "Okänt namn";

            var slot = await _context.Timeslots
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.Id == timeSlotId);

            if (slot == null || slot.Booking != null)
                return BadRequest("Tiden är redan bokad.");

            var booking = new Booking
            {
                TimeSlotId = timeSlotId,
                UserId = userId,
                CustomerEmail = userEmail!,
                CustomerName = userName,
                Status = "Booked",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("User") && booking.UserId != userId)
                return Forbid();

            ViewData["TimeSlotId"] = _context.Timeslots
                .Include(t => t.Booking)
                .Where(t => t.Booking == null || t.Id == booking.TimeSlotId)
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
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerEmail,Status,TimeSlotId")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Users får bara ändra sina egna bokningar
            if (User.IsInRole("User") && existingBooking.UserId != userId)
                return Forbid();

            // Kontrollera om ny tid är ledig
            var newSlot = await _context.Timeslots
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.Id == booking.TimeSlotId);

            if (newSlot == null || (newSlot.Booking != null && newSlot.Id != existingBooking.TimeSlotId))
            {
                ModelState.AddModelError("TimeSlotId", "Denna tid är redan bokad.");
            }

            if (ModelState.IsValid)
            {
                existingBooking.TimeSlotId = booking.TimeSlotId;

                // Admin kan ändra namn, email och status
                if (User.IsInRole("Admin"))
                {
                    existingBooking.CustomerName = booking.CustomerName;
                    existingBooking.CustomerEmail = booking.CustomerEmail;
                    existingBooking.Status = booking.Status;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["TimeSlotId"] = _context.Timeslots
                .Include(t => t.Booking)
                .Where(t => t.Booking == null || t.Id == existingBooking.TimeSlotId)
                .OrderBy(t => t.StartTime)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    Selected = t.Id == existingBooking.TimeSlotId
                }).ToList();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("User") && booking.UserId != userId)
                return Forbid();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> SelectTime(int coachId)
        {
            var times = await _context.Timeslots
                .Include(t => t.Coach)
                .Include(t => t.Booking)
                .Where(t =>
                    t.CoachId == coachId &&
                    t.Booking == null &&
                    t.StartTime > DateTime.Now)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            return View(times);
        }
    }
}