using s3839908_a2.Models;

namespace s3839908_a2.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        public Task InsertTransaction(Transaction transaction);
        public Task<List<Transaction>> GetTransactionsByAccount(int accountNumber);

    }
}
