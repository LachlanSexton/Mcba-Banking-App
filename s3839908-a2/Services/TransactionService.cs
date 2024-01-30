using s3839908_a2.Repositories.Interfaces;
using s3839908_a2.Services.Interfaces;
using Transaction = s3839908_a2.Models.Transaction;

namespace s3839908_a2.Services
{
    public class TransactionService(ITransactionRepository transactionRepository) : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task InsertTransaction(Transaction transaction)
        {
            await _transactionRepository.InsertTransaction(transaction);
        }

        public async Task<List<Transaction>> GetTransactionsByAccount(int accountNumber)
        {
            return await _transactionRepository.GetTransactionsByAccount(accountNumber);
        }

        public async Task<bool> IsFeeCharged(int accountNumber)
        {
            var sum = 0;
            //count withdrawals
            var transactions = await _transactionRepository.GetTransactionsByAccount(accountNumber);
            sum += transactions.Where(x => x.TransactionType == Enums.TransactionType.Withdraw).ToList().Count;
            //count outgoing transfers
            sum += transactions.Where(x => x.DestinationAccountNumber != null && x.TransactionType == Enums.TransactionType.Transfer).ToList().Count;

            return sum >= 2;
        }
    }
}
