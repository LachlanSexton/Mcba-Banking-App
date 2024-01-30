using Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Services;
using s3839908_a2.Services.Interfaces;
using System.Diagnostics;
using System.Security.Principal;

internal class BillPayScheduler : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Program> _logger;
    private Timer _timer;

    public BillPayScheduler(IServiceProvider serviceProvider, ILogger<Program> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Beginning scheduler");
        performCatchup();
        _timer = new Timer(async state => await ProcessPaymentsAsync(), null, CalculateInitialDelay(), TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping hosted service");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    static TimeSpan CalculateInitialDelay()
    {
        DateTime now = DateTime.UtcNow;
        DateTime nextScheduledTime = now.AddSeconds(2);
        return nextScheduledTime - now;
    }

    private async Task ProcessPaymentsAsync()
    {
        DateTime startTime = DateTime.Now;
        using (var scope = _serviceProvider.CreateScope())
        {
            var _mcbaContext = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            try
            {
                DateTime roundedNow = DateTime.UtcNow;
                var billPaysList = await _mcbaContext.BillPays.Where(bp => !bp.FailedBillPay).ToListAsync();

                foreach (var billPay in billPaysList)
                {
                    var difference = (billPay.ScheduleTimeUtc - roundedNow).TotalSeconds;

                    if (Math.Abs(difference) < 10) // Check if the time is within 10 seconds of the scheduled time
                    {
                        await processTransaction(billPay, _mcbaContext, _accountService);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payments.");
            }
            finally
            {
                _mcbaContext.Dispose();
            }
        }
    }



    private async void performCatchup()
    {
        _logger.LogInformation("Starting catchup");

        using (var scope = _serviceProvider.CreateScope())
        {
            var _mcbaContext = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var notProcessBillPays = _mcbaContext.BillPays.Where(bp => !bp.FailedBillPay && bp.ScheduleTimeUtc < DateTime.Now).ToList();

            foreach (var BillPay in notProcessBillPays)
            {
                _logger.LogInformation($"Processing billpay that needs catchup for BillPayId {BillPay.BillPayId}");
                await processTransaction(BillPay, _mcbaContext, _accountService);
            }
        }
        _logger.LogInformation("Finished Catchup");


    }

    private async Task processTransaction(BillPay billPay, McbaContext _mcbaContext, IAccountService accountService)
    {
        var account = _mcbaContext.Accounts.FirstOrDefault(a => a.AccountNumber == billPay.AccountNumber);
        if (account.Balance < billPay.Amount)
        {
            _logger.LogCritical("Failed BillPay");
            billPay.FailedBillPay = true;
        }
        else if(account.AccountType == AccountType.Checking && account.Balance - billPay.Amount < 300)
        {
            _logger.LogCritical("Failed BillPay due to balance of checking account under $300");
            billPay.FailedBillPay = true;
        }
        else
        {
            await accountService.BillPay(account, null, billPay.Amount, "BillPay");
            _mcbaContext.BillPays.Remove(billPay);
            _logger.LogCritical("BillPay Payment Complete");
        }

        if (billPay.Period == PeriodType.Monthly && !billPay.FailedBillPay)
        {
            _mcbaContext.BillPays.Add(makeMonthlyBillPay(billPay));
        }

        await _mcbaContext.SaveChangesAsync();
    }

    private BillPay makeMonthlyBillPay(BillPay billPay)
    {
        _logger.LogCritical("Making new BillPay for next month");
        BillPay BillPayNextMonth = new BillPay();
        BillPayNextMonth.PayeeID = billPay.PayeeID;
        BillPayNextMonth.AccountNumber = billPay.AccountNumber;
        BillPayNextMonth.Amount = billPay.Amount;
        BillPayNextMonth.Period = PeriodType.Monthly;
        BillPayNextMonth.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
        return BillPayNextMonth;
    }



}