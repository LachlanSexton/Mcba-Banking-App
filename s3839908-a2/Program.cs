using Data;
using Microsoft.EntityFrameworkCore;
using s3839908_a2.Repositories;
using s3839908_a2.Repositories.Interfaces;
using s3839908_a2.Services;
using s3839908_a2.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(McbaContext)));

    // Enable lazy loading.
    options.UseLazyLoadingProxies();
});

// Store session into Web-Server memory.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddHostedService<BillPayScheduler>();



var app = builder.Build();

// Seed data.
using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        await SeedData.InitializeAsync(services);
        var scheduler = app.Services.GetService<BillPayScheduler>();
        if (scheduler == null)
        {
            logger.LogError("BillPayScheduler not resolved.");
        }
    }
    catch(DbUpdateException ex)
    {
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapDefaultControllerRoute();


app.Run();


// Scheduler
// Read from billpay ID where time = time.now
// Remove billpay row 
// Add transaction
// if billpay date doesn't go through leave in table as error
// read from table where date does is in past to show error