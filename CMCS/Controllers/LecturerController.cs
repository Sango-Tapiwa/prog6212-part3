using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;
using CMCS.ViewModels;

namespace CMCS.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LecturerController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Where(c => c.UserId == userId)
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new ClaimSubmissionViewModel
            {
                LecturerName = user.FullName,
                HourlyRate = user.HourlyRate
            };
            model.ClaimItems.Add(new ClaimItemViewModel { HourlyRate = user.HourlyRate });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(ClaimSubmissionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var totalHours = model.ClaimItems.Where(i => !string.IsNullOrWhiteSpace(i.ActivityDescription))
                .Sum(i => i.HoursWorked);

            if (totalHours > 180)
            {
                ModelState.AddModelError("", "Total hours cannot exceed 180 hours per month.");
            }

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var existingMonthHours = await _context.ClaimItems
                .Where(ci => ci.Claim.UserId == userId.Value && ci.Date >= firstDayOfMonth)
                .SumAsync(ci => ci.HoursWorked);

            if (existingMonthHours + totalHours > 180)
            {
                ModelState.AddModelError("", $"You have already claimed {existingMonthHours} hours this month. Adding {totalHours} hours would exceed the 180-hour monthly limit.");
            }

            if (!ModelState.IsValid)
            {
                model.LecturerName = user.FullName;
                model.HourlyRate = user.HourlyRate;
                if (model.ClaimItems.Count == 0)
                    model.ClaimItems.Add(new ClaimItemViewModel { HourlyRate = user.HourlyRate });
                return View(model);
            }

            var claim = new Claim
            {
                UserId = userId.Value,
                SubmissionDate = DateTime.Now,
                Status = ClaimStatus.Submitted
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Add claim items
            foreach (var itemModel in model.ClaimItems.Where(i => !string.IsNullOrWhiteSpace(i.ActivityDescription)))
            {
                var claimItem = new ClaimItem
                {
                    ClaimId = claim.ClaimId,
                    Date = itemModel.Date,
                    HoursWorked = itemModel.HoursWorked,
                    HourlyRate = user.HourlyRate,
                    ActivityDescription = itemModel.ActivityDescription
                };
                _context.ClaimItems.Add(claimItem);
            }

            // Handle file uploads
            if (model.Documents != null && model.Documents.Any())
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                foreach (var file in model.Documents)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsPath, $"{Guid.NewGuid()}_{fileName}");

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var document = new Document
                        {
                            ClaimId = claim.ClaimId,
                            FileName = fileName,
                            FilePath = filePath,
                            FileSize = file.Length,
                            ContentType = file.ContentType,
                            UploadDate = DateTime.Now
                        };
                        _context.Documents.Add(document);
                    }
                }
            }

            // Calculate and update total
            await _context.SaveChangesAsync();
            claim.CalculateTotal();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim submitted successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewClaim(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var claim = await _context.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ClaimId == id && c.UserId == userId);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        public async Task<IActionResult> TrackClaims()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Where(c => c.UserId == userId)
                .Include(c => c.ClaimItems)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }
    }
}