using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this using
using CMCS.Data;
using CMCS.Models;
using CMCS.ViewModels;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user != null)
            {
                // Store user info in session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("UserName", user.FullName);

                // Redirect based on role
                return user.Role switch
                {
                    RoleType.Lecturer => RedirectToAction("Index", "Lecturer"),
                    RoleType.Coordinator => RedirectToAction("Index", "Coordinator"),
                    RoleType.Manager => RedirectToAction("Index", "Manager"),
                    RoleType.HR => RedirectToAction("Index", "HR"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}