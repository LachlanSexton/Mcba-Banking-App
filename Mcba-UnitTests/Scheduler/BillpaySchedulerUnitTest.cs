using System;
using System.Threading.Tasks;
using Data;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using s3839908_a2.Models;
using s3839908_a2.Services.Interfaces;
using Xunit;
using static System.Formats.Asn1.AsnWriter;

namespace Mcba_UnitTests.Scheduler
{
    public class BillpaySchedulerUnitTest
    {
        private readonly BillPayScheduler _billPayScheduler;
        private readonly Mock<McbaContext> _contextMock;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ILogger<Program>> _loggerMock;

        public BillpaySchedulerUnitTest()
        {
            _contextMock = new Mock<McbaContext>(new DbContextOptions<McbaContext>());
            _accountServiceMock = new Mock<IAccountService>();
            _loggerMock = new Mock<ILogger<Program>>();
        }
   


        [Fact]
        public async Task BillPayScheduler_HappyFlow()
        {

            // Act
            //await _billPayScheduler.StartAsync(CancellationToken.None);

        }


        private List<BillPay> makeBillpays(int timeInSeconds)
        {
            BillPay billpay = new BillPay()
            {
                PayeeID = 3,
                AccountNumber = 4100,
                Amount = 10000,
                ScheduleTimeUtc = DateTime.UtcNow.AddSeconds(timeInSeconds),
                Period = PeriodType.Monthly,
                FailedBillPay = false,
                Payee = new Payee()
                {
                    PayeeID = 3,
                    Name = "Test",
                    Address = "Test address",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "1234",
                    Phone = "040404040"
                }
            };
            BillPay billpay1 = new BillPay()
            {
                PayeeID = 4,
                AccountNumber = 4100,
                Amount = 10000,
                ScheduleTimeUtc = DateTime.UtcNow.AddSeconds(timeInSeconds),
                Period = PeriodType.Monthly,
                FailedBillPay = false,
                Payee = new Payee()
                {
                    PayeeID = 4,
                    Name = "Test",
                    Address = "Test address",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "1234",
                    Phone = "040404040"
                }
            };
            BillPay billpay2 = new BillPay()
            {
                PayeeID = 5,
                AccountNumber = 4100,
                Amount = 10000,
                ScheduleTimeUtc = DateTime.UtcNow.AddSeconds(timeInSeconds),
                Period = PeriodType.Monthly,
                FailedBillPay = false,
                Payee = new Payee()
                {
                    PayeeID = 5,
                    Name = "Test",
                    Address = "Test address",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "1234",
                    Phone = "040404040"
                }
            };

            Account account = new Account()
            {
                AccountNumber = 4100,
                AccountType = s3839908_a2.Enums.AccountType.Savings,
                Balance = 100000
            };

            List<BillPay> billPays = new List<BillPay>();
            billPays.Add(billpay);
            billPays.Add(billpay1);
            billPays.Add(billpay2);
            return billPays;
        }


    }
}
