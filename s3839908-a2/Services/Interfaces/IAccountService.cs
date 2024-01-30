using s3839908_a2.Enums;
using s3839908_a2.Models;

namespace s3839908_a2.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Account> GetAccount(int accountNumber);
        public Task<List<Account>> GetAccountsByCustomer(int customerId);
        public Task<TransactionResults> Deposit(Account account, decimal amount, string comment = null);
        public Task<TransactionResults> Withdraw(Account account, decimal amount, string comment = null);
        public Task<TransactionResults> Transfer(Account account, int destinationAccountNumber, decimal amount, string comment = null);
        public Task<TransactionResults> BillPay(Account sourceAccount, int? destinationAccountNumber, decimal amount, string comment = null);
    }
}
