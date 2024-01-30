using Data;
using McbaExample.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using s3839908_a2.Controllers;
using s3839908_a2.Models;
using s3839908_a2.Services;
using s3839908_a2.Services.Interfaces;
using s3839908_a2.ViewModels;
using SimpleHashing;
using SimpleHashing.Net;
using System.Transactions;
using Xunit;

namespace Mcba_Tests
{
    public class TransactionControllerUnitTest
    {
        private readonly TransactionController transactionController;
        private readonly Mock<McbaContext> _contextMock;
        private readonly Mock<AccountService> _accountService;


        public TransactionControllerUnitTest()
        {
            _contextMock = new Mock<McbaContext>(new DbContextOptions<McbaContext>());
            _accountService = new Mock<AccountService>();
            transactionController = new TransactionController(_contextMock.Object, _accountService.Object);
        }



    }


}