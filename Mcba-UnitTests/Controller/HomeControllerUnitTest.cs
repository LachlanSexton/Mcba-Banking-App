using Data;
using McbaExample.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Models;
using s3839908_a2.ViewModels;
using SimpleHashing;
using SimpleHashing.Net;
using System.Reflection.Metadata;

namespace Mcba_Tests
{
    public class HomeControllerUnitTest
    {
        private readonly HomeController homeController;
        private readonly McbaContext _context;
        ISimpleHash simpleHash = new SimpleHash();

        public HomeControllerUnitTest()
        {
            // Initialize test-specific resources
            var options = new DbContextOptionsBuilder<McbaContext>()
                .UseInMemoryDatabase("TestDatabase" + Guid.NewGuid())
                .Options;

            _context = new McbaContext(options);

            // Mock HttpContext and set up the session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();

            homeController = new HomeController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };
        }

        [Theory]
        [InlineData(null, "abc123")] // Test case with null login ID
        [InlineData("12345678", null)] // Test case with null password
        public async Task Login_Unhappyflow_nullLoginOrPassword(string loginId, string password)
        {
            // Arrange
            makeLogin("abc123", "12345678", false);
            await _context.SaveChangesAsync();

            // Act
            var result = await homeController.Login(loginId, password);

            // Assert
            Assert.True(homeController.ModelState.ContainsKey("LoginFailed")); // Ensure ModelState contains the error message
            var viewResult = result as ViewResult;
            Assert.Equal("Index", viewResult.ViewName); // Returns back to the index page

        }

        [Fact]
        public async Task Login_Unhappyflow_accountIsLocked()
        {
            // Arrange
            makeLogin("abc123", "12345678", true);
            await _context.SaveChangesAsync();

            // Act
            var result = await homeController.Login("abc123", "12345678");

            // Assert
            Assert.True(homeController.ModelState.ContainsKey("LoginFailed")); // Ensure ModelState contains the error message
            var viewResult = result as ViewResult;
            Assert.Equal("Index", viewResult.ViewName); // Returns back to the index page
        }

        [Fact]
        public async Task Login_HappyFlow()
        {
            // Arrange
            makeLogin("abc123", "12345678", false);
            await _context.SaveChangesAsync();

            // Act
            var result = await homeController.Login("abc123", "12345678");

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("Customer", redirectToActionResult.ControllerName); // Go to Customer Page

        }

        [Fact]
        public async Task Login_Happyflow_Logout()
        {
            // Arrange
            homeController.HttpContext.Session.SetInt32(nameof(Customer.CustomerID), 1);

            // Act
            var result = homeController.Logout();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.Equal("Home", redirectToActionResult.ControllerName); // Check if redirected to home page
            Assert.Null(homeController.HttpContext.Session.GetInt32(nameof(Customer.CustomerID))); // 

        }




        private void makeLogin(string loginId, string password, bool locked)
        {

            Login login = new Login
            {
                CustomerID = 1,
                PasswordHash = simpleHash.Compute(password),
                LoginID = loginId,
                Locked = locked,
                Customer = new Customer ()
                {
                    CustomerID = 1,
                    Name = "Lachie",
                    Address = "90 Croker st, Footscray",
                    TFN = "278656047",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Mobile = "0470708090",
                }
            };
            _context.Logins.Add(login);
        }

    }


}