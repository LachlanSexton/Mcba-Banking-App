using Data;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Models;
using s3839908_a2.Repositories.Interfaces;

namespace s3839908_a2.Repositories
{
    public class AccountRepository(McbaContext context) : IAccountRepository
    {
        private readonly McbaContext _context = context;

        public async Task<List<Account>> GetAccountsByCustomer(int customerId)
        {
            return await _context.Accounts.Where(acc => acc.CustomerID == customerId).ToListAsync();
        }

        public async Task<Account> GetAccount(int accountNumber)
        {
            return await _context.Accounts.Where(acc => acc.AccountNumber == accountNumber).FirstOrDefaultAsync();
        }

        public bool UpdateBalance(int accountNumber, decimal amount, bool increaseBalance)
        {            
                var account = context.Accounts
                    .Where(a => a.AccountNumber == accountNumber)
                    .FirstOrDefault();

                if (account != null)
                {
                    account.Balance += increaseBalance ? amount : -amount;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            
        }
    }
}
