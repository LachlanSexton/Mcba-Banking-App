using Data;
using McbaExample.Utilities;
using McbaExampleWithLogin.Filters;
using Microsoft.AspNetCore.Mvc;
using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Services;
using s3839908_a2.Services.Interfaces;
using s3839908_a2.ViewModels;
using SimpleHashing.Net;
using System.Drawing;
using X.PagedList;

namespace McbaExample.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly McbaContext _context;
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();
    private readonly IAccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    //Display Customer Accounts
    public async Task<IActionResult> Index()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        return View(customer);
    }


    //Render My Profile page pre populated with the available customer details
    public async Task<IActionResult> MyProfile()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        if(customer == null)
        {
            return NotFound();
        }

        var customerViewModel = new CustomerViewModel
        {
            Name = customer.Name,
            Address = customer.Address,
            TFN = customer.TFN,
            City = customer.City,
            State = customer.State,
            PostCode = customer.PostCode,
            Mobile = customer.Mobile,
        };

        return View(customerViewModel);
    }
    //Save updated profile
    [HttpPost]
    public async Task<IActionResult> SaveProfile(CustomerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("MyProfile", viewModel);
        }

        var customer = await _context.Customers.FindAsync(CustomerID);

        customer.Name = viewModel.Name;
        customer.Address = viewModel.Address;
        customer.TFN = viewModel.TFN;
        customer.State = viewModel.State;
        customer.PostCode = viewModel.PostCode;
        customer.Mobile = viewModel.Mobile;
        customer.City = viewModel.City;

        //Update customer name in the session
        HttpContext.Session.SetString(nameof(Customer.Name), customer.Name);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        if (!s_simpleHash.Verify(currentPassword,
            customer.Login.PasswordHash))
        {
            ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
        }

        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Password does not match new password.");
        }
        if (!ModelState.IsValid)
        {
            //Create Customer view Model to prefill customer profile form
            var customerViewModel = new CustomerViewModel
            {
                Name = customer.Name,
                TFN = customer.TFN,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                PostCode = customer.PostCode,
                Mobile = customer.Mobile
            };
            return View("MyProfile", customerViewModel);
        }

        //Hash new password
        customer.Login.PasswordHash = s_simpleHash.Compute(newPassword);

        await _context.SaveChangesAsync();

        return RedirectToAction("MyProfile");
    }

    //View statements, 4 per page
    public async Task<IActionResult> ViewStatements(int page = 1, int? selectedAccountId = null)
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        if (customer == null)
            return BadRequest();

        //Set default account id
        if (selectedAccountId == null)
        {
            selectedAccountId = customer.Accounts[0].AccountNumber;
        }

        ViewBag.Customer = customer;
        ViewBag.SelectedAccountId = selectedAccountId;

        const int pageSize = 4;

        var transactionsQuery = customer.Accounts
            .SelectMany(x => x.Transactions);

        if (selectedAccountId.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(x => x.AccountNumber == selectedAccountId.Value).OrderByDescending(x => x.TransactionTimeUtc);
        }

        var transactions = await transactionsQuery.ToPagedListAsync(page, pageSize);

        return View(transactions);
    }
}
