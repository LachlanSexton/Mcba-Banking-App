using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace s3839908_a2.ViewModels
{
    public class TransferViewModel : DepositOrWithdrawViewModel
    {

        [Required (ErrorMessage = "Please select a recipient account.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Account Number must contain only digits.")]
        public int? DestinationAccountId { get; set; }
    }
}
