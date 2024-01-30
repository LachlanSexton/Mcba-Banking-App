using Newtonsoft.Json;
using s3839908_a2.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace s3839908_a2.Models;


public class Transaction
{
    [Key]
    [DisplayName("Transaction ID")]
    public int TransactionID { get; set; }

    [Required]
    [Column("TransactionType", TypeName = "char")]
    public char TransactionTypeChar { get; set; } = 'D';

    [NotMapped]
    [DisplayName("Transaction Type")]
    public TransactionType TransactionType
    {
        get => (TransactionType)TransactionTypeChar;
        set => TransactionTypeChar = (char)value;
    }

    [ForeignKey("Account")]
    [DisplayName("Account Number")]

    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    [DisplayName("Destination Account Number")]

    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Required, NotNull]
    [Column(TypeName = "money")]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    public DateTime TransactionTimeUtc { get; set; }

    [NotMapped]
    [DisplayName("Transaction Time")]
    public string TransactionTimeLocal { get { return TransactionTimeUtc.ToLocalTime().ToString("g"); } }
}
