using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Web_App.Models;

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
public class Customer
{
    [RegularExpression(@"^\d{4}$", ErrorMessage = "CustomerID must be a 4-digit number.")]
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [RegularExpression(@"^\d{3}\s\d{3}\s\d{3}$", ErrorMessage = "TFN should be a number of the following format: XXX XXX XXX")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [RegularExpression(@"^[A-Za-z]{2,3}$", ErrorMessage = "State must be a two-letter or three-letter Australian state code.")]
    public string State { get; set; }

    [RegularExpression(@"^\d{4}$", ErrorMessage = "Post code must be a 4-digit number.")]
    public string PostCode { get; set; }

    [RegularExpression(@"^04\d{2}\s\d{3}\s\d{3}$", ErrorMessage = "Mobile must be in the format 04XX XXX XXX.")]
    public string Mobile { get; set; }
}

