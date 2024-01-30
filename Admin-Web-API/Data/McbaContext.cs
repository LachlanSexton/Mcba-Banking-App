using Microsoft.EntityFrameworkCore;
using Admin_Web_API.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.Data;

namespace Admin_Web_API.Data;

public class McbaContext : DbContext
{
    public McbaContext(DbContextOptions<McbaContext> options) : base(options)
    { }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Login> Logins { get; set; }
    
}
