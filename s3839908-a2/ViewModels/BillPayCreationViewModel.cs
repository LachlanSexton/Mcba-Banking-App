using s3839908_a2.Models;
using System.ComponentModel.DataAnnotations;

namespace s3839908_a2.ViewModels
{
    public class BillPayCreationViewModel
    {
        public BillPay BillPay { get; set; }

        public List<Account> Accounts { get; set; }

        [Required(ErrorMessage = "Please select an account.")]
        public int SelectedAccountId { get; set; }

        public List<Payee> Payees { get; set; }
    }
}
