public static class UserModuleExtensions
{
    public static IServiceCollection AddUserModule(this IServiceCollection services)
    {
        // Since this code is inside the same assembly/project, 
        // it can freely see the internal BrightspaceRepository
        services.AddScoped<IUserService, UserService>();

        // Register any other internal services, validators, or handlers here
        
        return services;
    }
}
