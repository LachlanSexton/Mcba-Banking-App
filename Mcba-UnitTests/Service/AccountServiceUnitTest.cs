using System;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Packaging;
using NuGet.Versioning;
using s3839908_a2.Enums;
using s3839908_a2.Models;
using s3839908_a2.Repositories;
using s3839908_a2.Repositories.Interfaces;
using s3839908_a2.Services;
using s3839908_a2.Services.Interfaces;
using Xunit;
using static System.Formats.Asn1.AsnWriter;

namespace Mcba_UnitTests.Service
{
    public class AccountServiceUnitTest
    {
        private readonly IAccountService accountService;
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<ITransactionService> transactionServiceMock;

        public AccountServiceUnitTest()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            transactionServiceMock = new Mock<ITransactionService>();
            accountService = new AccountService(transactionServiceMock.Object, accountRepositoryMock.Object);
        }

        [Fact]
        public async Task Deposit_Happyflow()
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);

            // Act
            var result = await accountService.Deposit(account, 50, "test");

            // Assert
            Assert.Equal(TransactionResults.Success, result);

            accountRepositoryMock.Verify(r => r.UpdateBalance(account.AccountNumber, 50m, true), Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                    tr => tr.AccountNumber == account.AccountNumber &&
                          tr.Amount == 50m &&
                          tr.TransactionType == TransactionType.Deposit &&
                          tr.Comment == "test")),
                Times.Once);
        }

        [Fact]
        public async Task Withdraw_Happyflow()
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);

            // Act
            var result = await accountService.Withdraw(account, 50, "test");

            // Assert
            Assert.Equal(TransactionResults.Success, result);

            accountRepositoryMock.Verify(r => r.UpdateBalance(account.AccountNumber, 50m, false), Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                    tr => tr.AccountNumber == account.AccountNumber &&
                          tr.Amount == 50m &&
                          tr.TransactionType == TransactionType.Withdraw &&
                          tr.Comment == "test")),
                Times.Once);
        }



        [Fact]
        public async Task Transfer_Happyflow()
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            CreateDummyCustomer(2345);
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);

            // Act
            var result = await accountService.Transfer(account,2345, 50, "test");

            // Assert
            Assert.Equal(TransactionResults.Success, result);

            accountRepositoryMock.Verify(r => r.UpdateBalance(account.AccountNumber, 50m, false), Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                    tr => tr.AccountNumber == account.AccountNumber &&
                          tr.Amount == 50m &&
                          tr.TransactionType == TransactionType.Transfer &&
                          tr.Comment == "test")),
                Times.Once);
        }

        [Fact]
        public async Task Transfer_Happyflow_ServiceCharge()
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            CreateDummyCustomer(2345);
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);
            transactionServiceMock.Setup(t => t.IsFeeCharged(account.AccountNumber)).ReturnsAsync(true);

            // Act
            var result = await accountService.Transfer(account, 2345, 50, "test");

            // Assert
            Assert.Equal(TransactionResults.Success, result);

            accountRepositoryMock.Verify(r => r.UpdateBalance(account.AccountNumber, 50.1m, false), Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                    tr => tr.AccountNumber == account.AccountNumber &&
                          tr.Amount == 50m &&
                          tr.TransactionType == TransactionType.Transfer &&
                          tr.Comment == "test")),
                Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                tr => tr.AccountNumber == account.AccountNumber &&
                      tr.Amount == 0.1m &&
                      tr.TransactionType == TransactionType.ServiceFee &&
                      tr.Comment == "test")),
                Times.Once);
        }

        [Fact]
        public async Task Withdraw_Happyflow_ServiceCharge()
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);
            transactionServiceMock.Setup(t => t.IsFeeCharged(account.AccountNumber)).ReturnsAsync(true);

            // Act
            var result = await accountService.Withdraw(account, 50, "test");

            // Assert
            Assert.Equal(TransactionResults.Success, result);

            accountRepositoryMock.Verify(r => r.UpdateBalance(account.AccountNumber, 50.05m, false), Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                    tr => tr.AccountNumber == account.AccountNumber &&
                          tr.Amount == 50m &&
                          tr.TransactionType == TransactionType.Withdraw &&
                          tr.Comment == "test")),
                Times.Once);

            transactionServiceMock.Verify(
                t => t.InsertTransaction(It.Is<Transaction>(
                tr => tr.AccountNumber == account.AccountNumber &&
                      tr.Amount == 0.05m &&
                      tr.TransactionType == TransactionType.ServiceFee &&
                      tr.Comment == "test")),
                Times.Once);
        }

        [Theory]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Withdraw_Unhappyflow_InsuffiencentFunds(int ammount)
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);
            transactionServiceMock.Setup(t => t.IsFeeCharged(account.AccountNumber)).ReturnsAsync(true);

            // Act
            var result = await accountService.Withdraw(account, ammount, "test");

            // Assert
            Assert.Equal(TransactionResults.SavingsLowerThanZero, result);
        }

        [Theory]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Transfer_Unhappyflow_InsuffiencentFunds(int amount)
        {
            // Arrange
            Account account = CreateDummyCustomer(1234).Accounts.First();
            CreateDummyCustomer(2345);

            accountRepositoryMock.Setup(r => r.GetAccount(It.IsAny<int>())).ReturnsAsync(account);
            transactionServiceMock.Setup(t => t.IsFeeCharged(account.AccountNumber)).ReturnsAsync(true);

            // Act
            var result = await accountService.Transfer(account, 2345, amount, "test");

            // Assert
            Assert.Equal(TransactionResults.SavingsLowerThanZero, result);
        }



        private Customer CreateDummyCustomer(int customerNumber)
        {
            return new Customer
            {
                CustomerID = customerNumber,
                Name = "John Doe",
                TFN = "123 456 789",
                Address = "123 Main Street",
                City = "Anytown",
                State = "VIC",
                PostCode = "3000",
                Mobile = "0412 345 678",
                Accounts = new List<Account>
            {
                new Account
                {
                    AccountNumber = customerNumber,
                    AccountType = AccountType.Savings,
                    Balance = 1000.00m
                }
            }
            };
        }

        private Transaction CreateDummyCustomerServiceFee(int TransactionId)
        {
            return new Transaction()
            {
                TransactionID = TransactionId,
                TransactionTimeUtc = DateTime.UtcNow,
                TransactionType = TransactionType.Transfer,
                AccountNumber = 1234,
                Amount = 10,
                Comment = "test",
                DestinationAccountNumber = null
            };
        }


    }
}
