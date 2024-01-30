using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using s3839908_a2.Models;

namespace s3839908_a2.ViewModels
{
    public enum AustralianStateCode
    {
        NSW,
        VIC,
        QLD,
        WA,
        SA,
        TAS,
        ACT,
        NT
    }
    public class CustomerViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        [RegularExpression(@"^\d{3}\s\d{3}\s\d{3}$", ErrorMessage = "TFN should be of the format: XXX XXX XXX")]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [RegularExpression(@"^[A-Za-z]{2,3}$", ErrorMessage = "State must be a two-letter or three-letter Australian state code.")]
        [EnumDataType(typeof(AustralianStateCode), ErrorMessage = "Invalid Australian state code.")]
        public string State { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Post code must be a 4-digit number.")]
        public string PostCode { get; set; }

        [RegularExpression(@"^04\d{2}\s\d{3}\s\d{3}$", ErrorMessage = "Mobile must be in the format 04XX XXX XXX.")]
        public string Mobile { get; set; }
    }
}
