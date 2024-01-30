using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Repositories.Interfaces;
using s3839908_a2.Services.Interfaces;

namespace s3839908_a2.Services
{
    public class AccountService : IAccountService
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountRepository _accountRepository;


        public AccountService(ITransactionService transactionService, IAccountRepository accountRepository)
        {
            _transactionService = transactionService;
            _accountRepository = accountRepository;
        }


        public async Task<Account> GetAccount(int accountNumber)
        {
            return await _accountRepository.GetAccount(accountNumber);
        }
        public async Task<List<Account>> GetAccountsByCustomer(int customerId)
        {
            return await _accountRepository.GetAccountsByCustomer(customerId);
        }

        public async Task<TransactionResults> Deposit(Account account, decimal amount, string comment = null)
        {
            return await PerformTransaction(account, null, amount, TransactionType.Deposit, comment);
        }

        public async Task<TransactionResults> Withdraw(Account account, decimal amount, string comment = null)
        {
            return await PerformTransaction(account, null, amount, TransactionType.Withdraw, comment);
        }

        public async Task<TransactionResults> Transfer(Account account, int destinationAccountNumber, decimal amount, string comment = null)
        {
            return await PerformTransaction(account, destinationAccountNumber, amount, TransactionType.Transfer, comment);
        }

        public async Task<TransactionResults> BillPay(Account account, int? destinationAccountNumber, decimal amount, string comment = null)
        {
            return await PerformTransaction(account, destinationAccountNumber, amount, TransactionType.BillPay, comment);
        }

        private async Task<TransactionResults> PerformTransaction(Account sourceAccount, int? destinationAccountNumber, decimal amount, TransactionType transactionType, string comment = null)
        {
            decimal feeAmount = 0;

            // Check if the balance is sufficient for the transaction
            if (transactionType != TransactionType.Deposit && ((sourceAccount.AccountType == AccountType.Savings && sourceAccount.Balance - amount < 0) ||
                (sourceAccount.AccountType == AccountType.Checking && sourceAccount.Balance - amount < 300)))
            {
                // Return the appropriate message based on account type
                return sourceAccount.AccountType == AccountType.Savings ? TransactionResults.SavingsLowerThanZero : TransactionResults.CheckingLowerThan300;
            }

            // Check if a fee should be charged
            if (transactionType != TransactionType.Deposit && await _transactionService.IsFeeCharged(sourceAccount.AccountNumber))
            {
                // Charge 0.05 for withdraws, 0.10 for transfers

                feeAmount += transactionType == Enums.TransactionType.Withdraw ? 0.05m : 0.10m;
                var serviceFeeTransaction = new Transaction
                {
                    TransactionType = TransactionType.ServiceFee,
                    AccountNumber = sourceAccount.AccountNumber,
                    Amount = feeAmount,
                    Comment = comment,
                    TransactionTimeUtc = DateTime.UtcNow
                };
                await _transactionService.InsertTransaction(serviceFeeTransaction);
            }

            var totalAmount = amount + feeAmount;
            // Update the source account balance
            _accountRepository.UpdateBalance(sourceAccount.AccountNumber, totalAmount, transactionType == TransactionType.Deposit);

            // Update the destination account balance if applicable
            if (destinationAccountNumber.HasValue)
            {
                _accountRepository.UpdateBalance(destinationAccountNumber.Value, totalAmount, true);
                var destinationTransaction = new Transaction
                {
                    TransactionType = transactionType,
                    AccountNumber = destinationAccountNumber.Value,
                    Amount = amount,
                    Comment = comment,
                    TransactionTimeUtc = DateTime.UtcNow
                };
                await _transactionService.InsertTransaction(destinationTransaction);
            }

            // Create a transaction object
            var transaction = new Transaction
            {
                TransactionType = transactionType,
                AccountNumber = sourceAccount.AccountNumber,
                DestinationAccountNumber = destinationAccountNumber,
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            };


            // Insert the transaction into the table
            await _transactionService.InsertTransaction(transaction);

            return TransactionResults.Success;
        }

    }
}
