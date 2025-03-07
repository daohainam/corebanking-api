
using CoreBanking.API.Models;
using CoreBanking.API.Services;

namespace CoreBanking.API.Apis;
public static class CoreBankingApi
{
    public static IEndpointRouteBuilder MapCoreBankingApi(this IEndpointRouteBuilder builder)
    {
        var vApi = builder.NewVersionedApi("CoreBanking");
        var v1 = vApi.MapGroup("api/v{version:apiVersion}/corebanking").HasApiVersion(1, 0);

        v1.MapGet("/customers", GetCustomers);
        v1.MapPost("/customers", CreateCustomer);

        v1.MapGet("/accounts", GetAccounts);
        v1.MapPost("/accounts", CreateAccount);
        v1.MapPut("/accounts/{id:guid}/deposit", Deposit);
        v1.MapPut("/accounts/{id:guid}/withdraw", Withdraw);
        v1.MapPut("/accounts/{id:guid}/transfer", Transfer);

        return builder;
    }

    private static async Task Transfer(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task Withdraw(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task Deposit(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task CreateAccount(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static async Task GetAccounts(
        [AsParameters] PaginationRequest pagination
        )
    {
        throw new NotImplementedException();
    }

    private static async Task CreateCustomer(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static async Task GetCustomers(
        [AsParameters] CoreBankingServices services,
        [AsParameters] PaginationRequest pagination
        )
    {
        throw new NotImplementedException();
    }
}
