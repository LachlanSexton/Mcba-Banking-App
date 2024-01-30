using System;
using System.Linq;
using Admin_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Admin_Web_API.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<McbaContext>();
            var customerRepository = serviceProvider.GetRequiredService<Repository<Customer>>();

            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.

            // Retrieve all customers from the remote Azure database
            var remoteCustomers = context.Customers.ToList();

            var remoteLogins = context.Logins.ToList();

            // Populate the local repository with remote customers
            foreach (var customer in remoteCustomers)
            {
                // Assuming Repository<Customer> has an "Add" method
                customerRepository.Add(customer);
            }
        }
    }
}