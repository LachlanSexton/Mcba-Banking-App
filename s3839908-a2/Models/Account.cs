using Newtonsoft.Json;
using s3839908_a2.Enums;
using s3839908_a2.Models.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3839908_a2.Models;

public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Account Number must be a 4-digit number.")]
    public int AccountNumber { get; set; }

    [JsonConverter(typeof(AccountTypeStringToAccountTypeEnumConverter))]
    [Display(Name = "Type")]
    [Required]
    public AccountType AccountType { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "money")]
    [Required]
    public decimal Balance { get; set; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    public virtual List<BillPay> BillPays { get; set; }
}
