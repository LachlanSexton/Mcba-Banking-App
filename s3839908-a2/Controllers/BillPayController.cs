using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Models;
using s3839908_a2.Services.Interfaces;
using s3839908_a2.ViewModels;

namespace s3839908_a2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;
        private readonly IAccountService _accountService;
        private readonly List<Payee> _payees;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public BillPayController(McbaContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
            _payees = _context.Payees.ToList();

        }

        //Render landing page for my billpays
        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            var billPayViewModel = new BillPayViewModel
            {
                Accounts = customer.Accounts,
            };

            return View(billPayViewModel);
        }

        //Render billpays for a specific account
        public async Task<IActionResult> BillPays(int selectedAccountId)
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            var account = customer.Accounts.Where(x => x.AccountNumber == selectedAccountId).FirstOrDefault();

            var viewModel = new BillPayViewModel
            {
                BillPays = account.BillPays,
                Accounts = customer.Accounts
            };

            return View("Index",viewModel);
        }

        //Render billpay creation page
        public async Task<IActionResult> BillPayCreation()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            if (customer == null)
                return NotFound();

            //Build view Model
            var payee = new Payee();
            var billpay = new BillPay { Payee = payee};
            var viewModel = new BillPayCreationViewModel
            {
                BillPay = billpay,
                Accounts = customer.Accounts,
                Payees = _payees
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBillPay(BillPayCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var accounts = _context.Accounts.Where(x => x.CustomerID == CustomerID).ToList();

                viewModel.Accounts = accounts;
                viewModel.Payees = _payees;
                return View("BillPayCreation", viewModel);
            }
            //Add account number to bill pay
            viewModel.BillPay.AccountNumber = viewModel.SelectedAccountId;

            var billpay = viewModel.BillPay;

            await _context.BillPays.AddAsync(billpay);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBillPay(int billPayId)
        {
            var billpay = await _context.BillPays.FindAsync(billPayId);
            if (billpay == null)
            {
                return NotFound();
            }
            _context.BillPays.Remove(billpay);

            _context.SaveChanges();

            //back to billpays page
            return RedirectToAction(nameof(BillPays), new { selectedAccountId = billpay.AccountNumber });
        }


    }
}
