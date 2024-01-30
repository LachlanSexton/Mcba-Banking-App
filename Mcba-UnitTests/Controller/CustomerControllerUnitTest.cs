using Data;
using McbaExample.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Models;
using s3839908_a2.ViewModels;
using SimpleHashing;
using SimpleHashing.Net;
using System.Net.Sockets;

namespace Mcba_Tests
{
    public class CustomerControllerUnitTest
    {
        private readonly CustomerController customerController;
        private readonly McbaContext _context;
        ISimpleHash simpleHash = new SimpleHash();

        public CustomerControllerUnitTest()
        {
            // Initialize test-specific resources
            var options = new DbContextOptionsBuilder<McbaContext>()
                .UseInMemoryDatabase("TestDatabase" + Guid.NewGuid())
                .Options;

            _context = new McbaContext(options);

            // Mock HttpContext and set up the session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            httpContext.Session.SetInt32(nameof(Customer.CustomerID), 1);



            customerController = new CustomerController(_context, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };
        }


        [Fact]
        public async Task myProfile_Happyflow()
        {
            Customer mockFoundCustomer = makeTestCustomer();
            _context.Customers.Add(mockFoundCustomer);
            await _context.SaveChangesAsync();

            // Act
            var result = await customerController.MyProfile();
            ViewResult viewResult = result as ViewResult;
            // Assert

            Assert.NotNull(viewResult); // Check if the result is a ViewResult
            Assert.IsType<CustomerViewModel>(viewResult.Model); // Check if the model is of type CustomerViewModel

            var model = viewResult.Model as CustomerViewModel;
            Assert.Equal(mockFoundCustomer.Name, model.Name);
            Assert.Equal(mockFoundCustomer.Address, model.Address);
            Assert.Equal(mockFoundCustomer.TFN, model.TFN);
            Assert.Equal(mockFoundCustomer.City, model.City);
            Assert.Equal(mockFoundCustomer.State, model.State);
            Assert.Equal(mockFoundCustomer.PostCode, model.PostCode);
            Assert.Equal(mockFoundCustomer.Mobile, model.Mobile);
        }


        [Fact]
        public async Task saveProfile_Happyflow()
        {
            Customer mockFoundCustomer = makeTestCustomer();
            _context.Customers.Add(mockFoundCustomer);
            await _context.SaveChangesAsync();

            CustomerViewModel customerViewModel = makeTestCustomerModelView();

            // Act
            var result = await customerController.SaveProfile(customerViewModel);

            // Assert
            var customerInDb = await _context.Customers.FindAsync(1);
            Assert.Equal(customerViewModel.Name, customerViewModel.Name);
            Assert.Equal(customerViewModel.Address, customerViewModel.Address);
        }







        [Fact]
        public async Task changeProfile_Happyflow()
        {
            Customer mockFoundCustomer = makeTestCustomer();
            _context.Customers.Add(mockFoundCustomer);
            await _context.SaveChangesAsync();

            // Act
            var result = await customerController.ChangePassword("abc123", "bcd234", "bcd234");


            // Assert
            RedirectToActionResult actionResult = result as RedirectToActionResult;
            var customerInDb = await _context.Customers.FindAsync(1);
            Assert.NotEqual(simpleHash.Compute("abc123"), customerInDb.Login.PasswordHash);
            Assert.Equal("MyProfile", actionResult.ActionName);
        }

        [Fact]
        public async Task changeProfile_Unhappyflow_PasswordsDoNotMatch()
        {
            Customer mockFoundCustomer = makeTestCustomer();
            _context.Customers.Add(mockFoundCustomer);
            await _context.SaveChangesAsync();

            // Act
            var result = await customerController.ChangePassword("abc123", "bcd234", "bcd235");

            // Assert
            ViewResult viewResult = result as ViewResult;
            var customerInDb = await _context.Customers.FindAsync(1);
            Assert.Equal(mockFoundCustomer.Login.PasswordHash, customerInDb.Login.PasswordHash);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("ConfirmPassword"));

            // Check if DB didn't update password
        }


        [Fact]
        public async Task changeProfile_Unhappyflow_WrongPassword()
        {
            Customer mockFoundCustomer = makeTestCustomer();
            _context.Customers.Add(mockFoundCustomer);
            await _context.SaveChangesAsync();

            // Act
            var result = await customerController.ChangePassword("wrongpassword", "bcd234", "bcd235");

            // Assert
            ViewResult viewResult = result as ViewResult;
            var customerInDb = await _context.Customers.FindAsync(1);
            Assert.Equal(mockFoundCustomer.Login.PasswordHash, customerInDb.Login.PasswordHash);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("CurrentPassword"));

            // Check if DB didn't update password
        }


        private Customer makeTestCustomer()
        {

            Customer testCustomer = new Customer
            {
                CustomerID = 1,
                Name = "Lachie",
                Address = "90 Croker st, Footscray",
                TFN = "278656047",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Mobile = "0470708090",
                Login = new Login()
                {
                    CustomerID = 1,
                    PasswordHash = simpleHash.Compute("abc123"),
                    LoginID = "12345678",
                    Locked = false
                }
            
            };

            return testCustomer;
        }

        private CustomerViewModel makeTestCustomerModelView()
        {

            CustomerViewModel testCustomer = new CustomerViewModel
            {
                Name = "Omar",
                Address = "45 Joe St",
                TFN = "278656047",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Mobile = "0470708090"
            };

            return testCustomer;
        }


    }


}