using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;

namespace CMCS.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var pendingClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview)
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .AsSplitQuery()
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            return View(pendingClaims);
        }

        public async Task<IActionResult> ReviewClaim(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var claim = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            if (claim.Status == ClaimStatus.Submitted)
            {
                claim.UpdateStatus(ClaimStatus.UnderReview);
                await _context.SaveChangesAsync();
            }

            return View(claim);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id, string? comments)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.UpdateStatus(ClaimStatus.Approved, comments);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id, string? comments)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.UpdateStatus(ClaimStatus.Rejected, comments);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim rejected successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllClaims()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .AsSplitQuery()
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }
    }
}
