using CoreBanking.API.Apis;
using CoreBanking.Infrastructure.Entity;
using System.Net.Http.Json;
using System.Security.Cryptography.Xml;

namespace CoreBanking.IntegrationTests.Tests
{
    public class IntegrationTest1
    {
        [Fact]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.CoreBanking_AppHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

            await using var app = await appHost.BuildAsync();
            var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
            await app.StartAsync();

            // Act
            var httpClient = app.CreateHttpClient("corebanking-api");
            await resourceNotificationService.WaitForResourceAsync("corebanking-api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));

            // start testing

            // Arrange
            var customer1 = new Customer()
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Address = "123 Main St"
            };

            var customer2 = new Customer()
            {
                Id = Guid.NewGuid(),
                Name = "Jane Smith",
                Address = "456 Elm St"
            };

            // Act
            var response1 = await httpClient.PostAsJsonAsync("api/v1/corebanking/customers", customer1);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            // Act
            var response2 = await httpClient.PostAsJsonAsync("api/v1/corebanking/customers", customer2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            // Arrange
            var account1 = new Account()
            {
                Id = Guid.NewGuid(),
                CustomerId = customer1.Id,
                Balance = 1000.00m
            };

            var account2 = new Account()
            {
                Id = Guid.NewGuid(),
                CustomerId = customer2.Id,
                Balance = 2000.00m
            };

            // Act
            response1 = await httpClient.PostAsJsonAsync("api/v1/corebanking/accounts", account1);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            // Act
            var getAccount1 = await response1.Content.ReadFromJsonAsync<Account>();

            // Assert
            Assert.NotNull(getAccount1);
            Assert.Equal(account1.Id, getAccount1.Id);
            Assert.Equal(account1.CustomerId, getAccount1.CustomerId);
            Assert.Equal(account1.Balance, getAccount1.Balance);
            Assert.NotEmpty(getAccount1.Number);

            // Act
            response2 = await httpClient.PostAsJsonAsync("api/v1/corebanking/accounts", account2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            // Act
            var getAccount2 = await response2.Content.ReadFromJsonAsync<Account>();

            // Assert
            Assert.NotNull(getAccount2);
            Assert.Equal(account2.Id, getAccount2.Id);
            Assert.Equal(account2.CustomerId, getAccount2.CustomerId);
            Assert.Equal(account2.Balance, getAccount2.Balance);
            Assert.NotEmpty(getAccount2.Number);

            // Act
            var getResponse1 = await httpClient.GetAsync($"api/v1/corebanking/customers/{customer1.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse1.StatusCode);

            // Act
            var getCustomer1 = await getResponse1.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(getCustomer1);
            Assert.Equal(customer1.Id, getCustomer1.Id);
            Assert.Equal(customer1.Name, getCustomer1.Name);
            Assert.Equal(customer1.Address, getCustomer1.Address);

            // Act
            var getResponse2 = await httpClient.GetAsync($"api/v1/corebanking/customers/{customer2.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse2.StatusCode);

            // Act
            var getCustomer2 = await getResponse2.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(getCustomer2);
            Assert.Equal(customer2.Id, getCustomer2.Id);
            Assert.Equal(customer2.Name, getCustomer2.Name);
            Assert.Equal(customer2.Address, getCustomer2.Address);

            // Act
            response1 = await httpClient.PutAsJsonAsync($"api/v1/corebanking/accounts/{account1.Id}/deposit", new DepositionRequest() {
                Amount = 50000.00m
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            // Act
            response1 = await httpClient.PutAsJsonAsync($"api/v1/corebanking/accounts/{account2.Id}/withdraw", new WithdrawalRequest()
            {
                Amount = 5000.00m
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);

            // Act
            response2 = await httpClient.PutAsJsonAsync($"api/v1/corebanking/accounts/{account2.Id}/withdraw", new WithdrawalRequest()
            {
                Amount = 1999.00m
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            // Act
            response1 = await httpClient.PutAsJsonAsync($"api/v1/corebanking/accounts/{account1.Id}/transfer", new TransferRequest()
            {
                Amount = 100000.00m,
                DestinationAccountNumber = getAccount2.Number
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);

            // Act
            response1 = await httpClient.PutAsJsonAsync($"api/v1/corebanking/accounts/{account1.Id}/transfer", new TransferRequest()
            {
                Amount = 51000.00m,
                DestinationAccountNumber = getAccount2.Number
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            // Act
            response1 = await httpClient.GetAsync($"api/v1/corebanking/accounts/{getAccount1.Number}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            // Act
            getAccount1 = await response1.Content.ReadFromJsonAsync<Account>();

            // Assert
            Assert.NotNull(getAccount1);
            Assert.Equal(account1.Id, getAccount1.Id);
            Assert.Equal(0, getAccount1.Balance);

            // Act
            response2 = await httpClient.GetAsync($"api/v1/corebanking/accounts/{getAccount2.Number}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            // Act
            getAccount2 = await response2.Content.ReadFromJsonAsync<Account>();

            // Assert
            Assert.NotNull(getAccount2);
            Assert.Equal(account2.Id, getAccount2.Id);
            Assert.Equal(51001.00m, getAccount2.Balance);
        }
    }
}
