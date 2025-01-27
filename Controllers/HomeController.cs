using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharingSystem.Models;
using RideSharingSystem.Data;

namespace RideSharingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Make sure to include the Driver information and get the latest rides
                var rides = _context.Rides
                    .Include(r => r.Driver)  // This ensures we load the Driver data
                    .OrderByDescending(r => r.Id)  // This shows newest rides first
                    .ToList();

                // Add some debug logging
                _logger.LogInformation($"Retrieved {rides.Count} rides");
                foreach (var ride in rides)
                {
                    _logger.LogInformation($"Ride ID: {ride.Id}, From: {ride.StartLocation}, To: {ride.EndLocation}, Driver: {ride.DriverId}");
                }

                return View(rides);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving rides: {ex.Message}");
                return View(new List<Ride>());
            }
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string StartLocation, string EndLocation, int DriverId, int AvailableSeats, decimal Price)
        {
            try
            {
                // Create new ride manually from form fields
                var ride = new Ride
                {
                    StartLocation = StartLocation,
                    EndLocation = EndLocation,
                    DriverId = DriverId,
                    AvailableSeats = AvailableSeats,
                    Price = Price
                };

                // Debug info
                System.Diagnostics.Debug.WriteLine($"Creating ride: {StartLocation} to {EndLocation}, Driver: {DriverId}, Seats: {AvailableSeats}, Price: {Price}");

                // Add and save
                _context.Rides.Add(ride);
                await _context.SaveChangesAsync();

                // Redirect to index
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to create ride: " + ex.Message);
                return View();
            }
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        // GET: Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }
            return View(ride);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string StartLocation, string EndLocation,
            int DriverId, int AvailableSeats, decimal Price)
        {
            try
            {
                var ride = await _context.Rides.FindAsync(id);
                if (ride == null)
                {
                    return NotFound();
                }

                // Update the ride properties
                ride.StartLocation = StartLocation;
                ride.EndLocation = EndLocation;
                ride.DriverId = DriverId;
                ride.AvailableSeats = AvailableSeats;
                ride.Price = Price;

                _context.Update(ride);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating ride: " + ex.Message);
                return View();
            }
        }

        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ride = await _context.Rides.FindAsync(id);
            if (ride != null)
            {
                _context.Rides.Remove(ride);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
    }
}