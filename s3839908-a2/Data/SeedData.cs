using Newtonsoft.Json;
using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Models.Converters;

namespace Data;

public static class SeedData
{
    //This could be async to load UI and seed db simultaneously
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();

        // Look for customers.
        if (context.Customers.Any())
            return; // DB has already been seeded.

        var dataUrl = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        using var client = new HttpClient();
        var json = await client.GetStringAsync(dataUrl);

        var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Converters = { new AccountTypeStringToAccountTypeEnumConverter() }
        });

        foreach (var customer in customers)
        {
            var accounts = customer.Accounts;
            foreach (var account in accounts)
            {
                account.Balance = account.Transactions.Sum(x => x.Amount);
            }
        }

        context.Customers.AddRange(customers);

        var payees = new List<Payee>
        {
            new Payee
            {
                Name = "Telstra",
                Address = "1 Bourke St",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Phone = "(04) 1234 5678"
            },
            new Payee
            {
                Name = "Optus",
                Address = "2 Bourke St",
                City = "Brisbane",
                State = "QLD",
                PostCode = "2000",
                Phone = "(04) 1234 5679"
            },
            new Payee
            {
                Name = "Vodafone",
                Address = "3 Bourke St",
                City = "Perth",
                State = "WA",
                PostCode = "1000",
                Phone = "(04) 1234 5670"
            }
        };

        context.Payees.AddRange(payees);

        await context.SaveChangesAsync();
    }

}
