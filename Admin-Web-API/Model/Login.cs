﻿using Admin_Web_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Web_API.Models;

public class Login
{
    [Column(TypeName = "char")]
    [StringLength(8)]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(94)]
    public string PasswordHash { get; set; }

    public bool locked { get; set; }
}
