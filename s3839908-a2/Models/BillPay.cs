using McbaExample.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3839908_a2.Models
{

    public enum PeriodType
    {
        OneOff = 1,
        Monthly = 2
    }
    public class BillPay
    {
        [Key]
        [Display(Name = "BillPay ID")]
        public int BillPayId { get; set; }

        [ForeignKey(nameof(Account))]
        [Display(Name ="Account Number")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey(nameof(Payee))]
        [Display(Name = "Payee ID")]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount should be a positive number.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduleTimeUtc { get; set; }

        [NotMapped]
        [Display(Name ="Scheduled Time")]
        public string ScheduleTimeLocal
        {
            get { return ScheduleTimeUtc.ToLocalTime().ToString("g"); }
            set
            {
                if (value != null)
                    ScheduleTimeUtc = DateTime.Parse(value).ToUniversalTime();
            }
        }

        [Required]
        [Column(TypeName = "char")]
        public PeriodType Period { get; set; }

        [Column(TypeName = "bit")]
        public bool FailedBillPay { get; set; }

    }
}
