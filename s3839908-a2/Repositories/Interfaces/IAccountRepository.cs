using s3839908_a2.Models;

namespace s3839908_a2.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<List<Account>> GetAccountsByCustomer(int customerId);
        public Task<Account> GetAccount(int accountNumber);
        public bool UpdateBalance(int accountNumber, decimal amount, bool increaseBalance);
    }
}
