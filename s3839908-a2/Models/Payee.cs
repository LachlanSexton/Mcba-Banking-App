using System.ComponentModel.DataAnnotations;

namespace s3839908_a2.Models
{
    public class Payee
    {
        [Key]
        public int PayeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string City { get; set; }

        [Required]
        [StringLength(3)]
        [RegularExpression(@"^[A-Za-z]{2,3}$", ErrorMessage = "State must be a two-letter or three-letter Australian state code.")]
        public string State { get; set; }

        [Required]
        [StringLength(4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Post code must be a 4-digit number.")]
        public string PostCode { get; set; }

        [Required]
        [StringLength(14)]
        [RegularExpression(@"^\(0\d\)\s\d{4}\s\d{4}$", ErrorMessage = "Phone must be in the format 0XXX XXX XXX.")]
        public string Phone { get; set; }
    }
}
