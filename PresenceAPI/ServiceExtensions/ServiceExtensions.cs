using domain.UseCase;
using data.Repository;

public static class ServiceExtensions
{
    public static void ConfigurateRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGroupRepository, SQLGroupRepository>();
        services.AddScoped<IUserRepository, SQLUserRepository>();
        services.AddScoped<IPresenceRepository, SQLPresenceRepository>();
    }

    public static void ConfigurateGroupUseCase(this IServiceCollection services)
    {
        services.AddScoped<GroupUseCase>();
    }

    public static void ConfigurateUserUseCase(this IServiceCollection services)
    {
        services.AddScoped<UserUseCase>();
    }

    public static void ConfiguratePresenceUseCase(this IServiceCollection services)
    {
        services.AddScoped<PresenceUseCase>();
    }
    

    public static void ConfigurateAdminPanel(this IServiceCollection services)
    {
        services.ConfigurateRepositories();
        
        services.AddScoped<GroupUseCase>();
        services.AddScoped<UserUseCase>();
        services.AddScoped<PresenceUseCase>();
    }
}