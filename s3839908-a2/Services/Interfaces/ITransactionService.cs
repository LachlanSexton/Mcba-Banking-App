using s3839908_a2.Models;

namespace s3839908_a2.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<List<Transaction>> GetTransactionsByAccount(int accountNumber);
        public Task InsertTransaction(Transaction transaction);
        public Task<bool> IsFeeCharged(int accountNumber);
    }
}