using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3839908_a2.Models;

public class Login
{
    [Column(TypeName = "char")]
    [Required, StringLength(8)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string LoginID { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerID { get; set; }

    public virtual Customer Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(94)]
    public string PasswordHash { get; set; }

    [Column(TypeName = "bit")]
    public bool Locked { get; set; }
}
