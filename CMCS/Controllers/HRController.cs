using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;
using CMCS.ViewModels;

namespace CMCS.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            var users = await _context.Users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            return View(new UserManagementViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserManagementViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password,
                Role = model.Role,
                FirstName = model.FirstName,
                LastName = model.LastName,
                HourlyRate = model.HourlyRate,
                IsActive = model.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {user.FullName} created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserManagementViewModel
            {
                UserId = user.UserId,
                Email = user.Email,
                Password = user.PasswordHash,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                HourlyRate = user.HourlyRate,
                IsActive = user.IsActive
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserManagementViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound();

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != model.UserId);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Another user with this email already exists.");
                return View(model);
            }

            user.Email = model.Email;
            user.PasswordHash = model.Password;
            user.Role = model.Role;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.HourlyRate = model.HourlyRate;
            user.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {user.FullName} updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {user.FullName} deactivated successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reports()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "HR")
                return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }
    }
}
