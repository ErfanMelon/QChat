using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QChat.Application.Interfaces;
using QChat.Persistance.Context;

namespace QChat.Persistance;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatDbContext>(option =>
        option.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IChatDbContext, ChatDbContext>();
        return services;
    }
}
