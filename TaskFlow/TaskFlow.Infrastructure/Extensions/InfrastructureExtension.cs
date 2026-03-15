using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Persistence.Context;
using TaskFlow.Infrastructure.Persistence.Repositories;
using TaskFlow.Infrastructure.Realtime;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DapperConfig();
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();

        });
       
       //current user
       services.AddScoped<ICurrentUser, CurrentUser>();
       
       //repos
       services.AddScoped<IBoardRepository, BoardRepository>();
       services.AddScoped<IColumnRepository, ColumnRepository>();
       services.AddScoped<ITaskRepository, TaskRepository>();
       
       //service
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IColumnService, ColumnService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        
        //signalR
        services.AddSignalR();
        services.AddScoped<IBoardNotifier, BoardNotifier>();

        return services;
    }

    private static void DapperConfig()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}