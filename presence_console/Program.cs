using data.RemoteData.RemoteDataBase;
using data.Repository;
using domain.UseCase;
using Microsoft.Extensions.DependencyInjection;
using ui;


IServiceCollection services = new ServiceCollection();

services
    .AddDbContext<RemoteDatabaseContext>()
    .AddSingleton<IGroupRepository, SQLGroupRepository>()
    .AddSingleton<IUserRepository, SQLUserRepository>()
    .AddSingleton<IPresenceRepository, SQLPresenceRepository>()
    .AddSingleton<UserUseCase>()
    .AddSingleton<GroupUseCase>()
    .AddSingleton<PresenceUseCase>()
    .AddSingleton<MainMenuUI>();



var serviceProvider = services.BuildServiceProvider();
MainMenuUI mainMenuUI = serviceProvider.GetService<MainMenuUI>();
mainMenuUI.Start();