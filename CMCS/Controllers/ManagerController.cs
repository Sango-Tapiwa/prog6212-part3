using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;

namespace CMCS.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var totalClaims = await _context.Claims.CountAsync();
            var pendingClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview);
            var approvedClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Approved);
            var rejectedClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Rejected);
            var totalApprovedAmount = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved)
                .SumAsync(c => c.TotalAmount);

            ViewBag.TotalClaims = totalClaims;
            ViewBag.PendingClaims = pendingClaims;
            ViewBag.ApprovedClaims = approvedClaims;
            ViewBag.RejectedClaims = rejectedClaims;
            ViewBag.TotalApprovedAmount = totalApprovedAmount;

            var recentClaims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .AsSplitQuery()
                .OrderByDescending(c => c.SubmissionDate)
                .Take(10)
                .ToListAsync();

            return View(recentClaims);
        }

        public async Task<IActionResult> Reports()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Manager")
                return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .AsSplitQuery()
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        public async Task<IActionResult> PendingClaims()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Manager")
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
    }
}