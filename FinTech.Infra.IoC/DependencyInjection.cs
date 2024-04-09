using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinTech.Infra.Data.Context;
using FinTech.Domain.Interfaces;
using FinTech.Infra.Data.Repositories;

namespace FinTech.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName: "AuthorDb")
        //options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
        //b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
        );

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryGroupRepository, CategoryGroupRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
