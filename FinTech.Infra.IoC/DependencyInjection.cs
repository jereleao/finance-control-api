using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinTech.Infra.Data.Context;
using FinTech.Domain.Interfaces;
using FinTech.Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using FinTech.Domain.Entities;

namespace FinTech.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = Environment.GetEnvironmentVariable("MYSQL_CONN_STRING") ?? string.Empty;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryGroupRepository, CategoryGroupRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
