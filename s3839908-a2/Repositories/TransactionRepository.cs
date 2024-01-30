using Data;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Models;
using s3839908_a2.Repositories.Interfaces;

namespace s3839908_a2.Repositories
{
    public class TransactionRepository(McbaContext context) : ITransactionRepository
    {
        private readonly McbaContext _context = context;

        public async Task InsertTransaction(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByAccount(int accountNumber)
        {
            return await _context.Transactions.Where(t => t.AccountNumber == accountNumber).ToListAsync();
        }

    }
}
