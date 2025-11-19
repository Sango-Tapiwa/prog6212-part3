using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using CMCS.Controllers;
using CMCS.Data;
using CMCS.Models;
using CMCS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMCS.Tests.Controllers
{
    public class AccountControllerTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CMCS_Test_DB")
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed test data
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { UserId = 1, Email = "lecturer@university.edu", PasswordHash = "lecturer123", Role = RoleType.Lecturer, FirstName = "John", LastName = "Smith" },
                    new User { UserId = 2, Email = "coordinator@university.edu", PasswordHash = "coordinator123", Role = RoleType.Coordinator, FirstName = "Jane", LastName = "Doe" }
                );
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToCorrectRolePage()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            // Mock HttpContext and Session
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            var model = new LoginViewModel
            {
                Email = "lecturer@university.edu",
                Password = "lecturer123"
            };

            // Act
            var result = await controller.Login(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Lecturer", result.ControllerName);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new LoginViewModel
            {
                Email = "wrong@university.edu",
                Password = "wrongpassword"
            };

            // Act
            var result = await controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ErrorCount > 0);
        }

        [Fact]
        public void Logout_ClearsSession_RedirectsToLogin()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            // Mock HttpContext and Session
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = controller.Logout() as RedirectToActionResult;

            // Assert - FIXED: When redirecting within same controller, ControllerName is null
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Null(result.ControllerName); // This is correct - same controller
        }

        [Fact]
        public async Task Login_WithCoordinatorCredentials_RedirectsToCoordinatorPage()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            var model = new LoginViewModel
            {
                Email = "coordinator@university.edu",
                Password = "coordinator123"
            };

            // Act
            var result = await controller.Login(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Coordinator", result.ControllerName);
        }

        [Fact]
        public async Task Login_WithManagerCredentials_RedirectsToManagerPage()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();

            // Add manager user for this test
            context.Users.Add(new User
            {
                UserId = 3,
                Email = "manager@university.edu",
                PasswordHash = "manager123",
                Role = RoleType.Manager,
                FirstName = "Bob",
                LastName = "Johnson"
            });
            context.SaveChanges();

            var controller = new AccountController(context);

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            var model = new LoginViewModel
            {
                Email = "manager@university.edu",
                Password = "manager123"
            };

            // Act
            var result = await controller.Login(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Manager", result.ControllerName);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ReturnsView()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            // Create invalid model
            controller.ModelState.AddModelError("Email", "Email is required");

            var model = new LoginViewModel
            {
                Email = "",
                Password = ""
            };

            // Act
            var result = await controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Logout_VerifiesSessionClearCalled()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var controller = new AccountController(context);

            // Create a mock session that tracks if Clear was called
            var sessionData = new Dictionary<string, byte[]>();
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.Clear()).Callback(() => sessionData.Clear());

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = controller.Logout();

            // Assert
            mockSession.Verify(s => s.Clear(), Times.Once);
        }
    }
}