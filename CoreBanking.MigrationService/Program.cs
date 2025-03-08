using CoreBanking.Infrastructure.Data;
using CoreBanking.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.AddNpgsqlDbContext<CoreBankingDbContext>("corebanking-db", configureDbContextOptions: dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(CoreBankingDbContext).Assembly.FullName));
}
);

var host = builder.Build();
host.Run();
