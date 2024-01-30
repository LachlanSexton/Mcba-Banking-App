using Data;
using Microsoft.AspNetCore.Mvc;
using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Services.Interfaces;
using s3839908_a2.ViewModels;

namespace s3839908_a2.Controllers
{
    //Controller that handles Deposit, Withdraw and Transfer
    public class TransactionController : Controller
    {
        private readonly McbaContext _context;
        private readonly IAccountService _accountService;


        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
        public TransactionController(McbaContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        //Render transaction page based on transactionType
        public async Task<IActionResult> Index(TransactionType transactionType)
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            var viewModel = new DepositOrWithdrawViewModel
            {
                Accounts = customer.Accounts,
                TransactionType = transactionType,
            };

            return View(viewModel);
        }

        //Perform deposit
        [HttpPost]
        public async Task<IActionResult> Deposit(DepositOrWithdrawViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(viewModel, TransactionType.Deposit);
            }
            var account = await _accountService.GetAccount(viewModel.SelectedAccountId.Value);


            await _accountService.Deposit(account, viewModel.Amount.GetValueOrDefault(), viewModel.Comment);

            return RedirectToAction(nameof(Index), nameof(Customer));
        }

        //Perform Withdraw
        [HttpPost]
        public async Task<IActionResult> Withdraw(DepositOrWithdrawViewModel viewModel)
        {
            //check if input is valid
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(viewModel, TransactionType.Withdraw);
            }
            var account = await _accountService.GetAccount(viewModel.SelectedAccountId.GetValueOrDefault());

            var transactionResult = await _accountService.Withdraw(account, viewModel.Amount.GetValueOrDefault(), viewModel.Comment);

            HandleWithdrawalResults(transactionResult);

            //check if withdraw was valid
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(viewModel, TransactionType.Withdraw);

            }

            return RedirectToAction(nameof(Index), nameof(Customer));
        }


        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel viewModel)
        {
            //check if input is valid
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(viewModel, TransactionType.Transfer);
            }

            var destAccount = await _accountService.GetAccount(viewModel.DestinationAccountId.GetValueOrDefault());
            var account = await _accountService.GetAccount(viewModel.SelectedAccountId.GetValueOrDefault());

            var transactionResult = await _accountService.Transfer(account, viewModel.DestinationAccountId.GetValueOrDefault(), viewModel.Amount.GetValueOrDefault(), viewModel.Comment);

            HandleWithdrawalResults(transactionResult);

            //check if input is valid
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(viewModel, TransactionType.Transfer);
            }

            return RedirectToAction(nameof(Index), nameof(Customer));

        }

        public void HandleWithdrawalResults(TransactionResults transactionResult)
        {
            switch (transactionResult)
            {
                case TransactionResults.SavingsLowerThanZero:
                    ModelState.AddModelError("Amount", "Savings cannot be lower than zero.");
                    break;

                case TransactionResults.CheckingLowerThan300:
                    ModelState.AddModelError("Amount", "Checking cannot be lower than $300.");
                    break;
            }
        }
        private async Task<IActionResult> HandleInvalidModelState(DepositOrWithdrawViewModel viewModel, TransactionType transactionType)
        {
            //get accounts to display them again
            var accounts = await _accountService.GetAccountsByCustomer(CustomerID);
            viewModel.Accounts = accounts;
            viewModel.TransactionType = transactionType;
            return View(nameof(Index), viewModel);
        }
    }
}
