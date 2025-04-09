using CoreBanking.API.Apis;
using CoreBanking.API.Services;
using CoreBanking.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoreBanking.UnitTests
{
    public class CoreBankingUnitTests
    {
        private SqliteConnection _sqliteConnection = default!;
        private DbContextOptions<CoreBankingDbContext> _dbContextOptions = default!;

        [Fact]
        public void Create_Customer_UnitTest()
        {
            // Arrange
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<CoreBankingDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            using (var context = new CoreBankingDbContext(_dbContextOptions))
            {
                context.Database.EnsureCreated();

                var services = new CoreBankingServices(context, NullLogger<CoreBankingServices>.Instance);

                var customer = new Infrastructure.Entity.Customer()
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    Address = "123 Main St",
                    Accounts = []
                };

                // Act
                var result = CoreBankingApi.CreateCustomer(services, customer);

                // Assert
                Assert.NotNull(result);

                // Verify that the customer was added to the database
                var addedCustomer = context.Customers.FirstOrDefault(c => c.Id == customer.Id);
                Assert.NotNull(addedCustomer);
                Assert.Equal(customer.Name, addedCustomer.Name);
                Assert.Equal(customer.Address, addedCustomer.Address);
                Assert.Equal(customer.Accounts.Count, addedCustomer.Accounts.Count);
            }
        }
    

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        [InlineData(9999999999)]
        [InlineData(9999999999.99999)]
        public void Create_Customer_And_Deposit_UnitTest(decimal depositAmount)
        {
            // Arrange
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();
            _dbContextOptions = new DbContextOptionsBuilder<CoreBankingDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            using var context = new CoreBankingDbContext(_dbContextOptions);
            context.Database.EnsureCreated();

            var services = new CoreBankingServices(context, NullLogger<CoreBankingServices>.Instance);

            var customer = new Infrastructure.Entity.Customer()
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Address = "123 Main St",
                Accounts = []
            };

            // Act
            var result = CoreBankingApi.CreateCustomer(services, customer);

            // Assert
            Assert.NotNull(result);

            // Verify that the customer was added to the database
            var addedCustomer = context.Customers.FirstOrDefault(c => c.Id == customer.Id);
            Assert.NotNull(addedCustomer);

            // Create an account for the customer
            var account = new Infrastructure.Entity.Account()
            {
                Id = Guid.NewGuid(),
                CustomerId = addedCustomer.Id,
                Balance = 0
            };

            var accountResult = CoreBankingApi.CreateAccount(services, account);

            // Assert that the account was created successfully
            Assert.NotNull(accountResult);

            // Deposit money into the account
            var depositResult = CoreBankingApi.Deposit(services, account.Id, new DepositionRequest() { 
                Amount = depositAmount
            });

            // Assert that the deposit was successful
            Assert.NotNull(depositResult);

            // Verify that the account balance was updated
            var updatedAccount = context.Accounts.FirstOrDefault(a => a.Id == account.Id);
            Assert.NotNull(updatedAccount);
            Assert.Equal(depositAmount, updatedAccount.Balance);

            var transaction = context.Transactions.Where(t => t.AccountId == account.Id);
            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.Count());
            Assert.Equal(depositAmount, transaction.First().Amount);
        }
    }
}
