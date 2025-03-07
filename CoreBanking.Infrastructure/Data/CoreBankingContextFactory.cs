using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreBanking.Infrastructure.Data;
public class CoreBankingDbContextFactory : IDesignTimeDbContextFactory<CoreBankingDbContext>
{
    public CoreBankingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoreBankingDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=corebanking;Username=postgres;Password=postgres");

        return new CoreBankingDbContext(optionsBuilder.Options);
    }
}
