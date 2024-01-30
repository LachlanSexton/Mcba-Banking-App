using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using s3839908_a2.Models;
using s3839908_a2.Enums;

namespace s3839908_a2.ViewModels
{
    public class DepositOrWithdrawViewModel
    {

        [Required(ErrorMessage = "Please select an account.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Account Number must contain only digits.")]
        public int? SelectedAccountId { get; set; }
        [Required(ErrorMessage = "Please enter an amount.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Amount must contain only digits.")]
        public decimal? Amount { get; set; }

        public string Comment { get; set; }

        public decimal? Balance { get; set; }

        public List<Account> Accounts { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}
