using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Infrastructure.Api;
using Modules.Users.Infrastructure.Authorization;
using Refit;
using Syncfusion.Maui.Toolkit.Hosting;
using WorkoutLogg.Database;
using AuthService = Modules.Users.Infrastructure.Authorization.AuthService;

namespace WorkoutLogg;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.ConfigureMauiHandlers(handlers =>
			{
#if WINDOWS
				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
				{
					handler.PlatformView.SingleSelectionFollowsFocus = false;
				});

				Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) =>
				{
					if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
					{
						contentPanel.IsTabStop = true;
					}
				});
#endif
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
			});

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

		builder.Services.AddSingleton<ProjectRepository>();
		builder.Services.AddSingleton<TaskRepository>();
		builder.Services.AddSingleton<CategoryRepository>();
		builder.Services.AddSingleton<TagRepository>();
		builder.Services.AddSingleton<SeedDataService>();
		builder.Services.AddSingleton<ModalErrorHandler>();
		builder.Services.AddSingleton<MainPageModel>();
		builder.Services.AddSingleton<ProjectListPageModel>();
		builder.Services.AddSingleton<ManageMetaPageModel>();

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddScoped<IAuthFlow, AuthFlow>();
        builder.Services.AddTransient<IAuthService, AuthService>();
		builder.Services.AddTransient<AuthHeaderHandler>();

		builder.Services.AddSingleton<WorkoutDatabase>();
        builder.Services.AddSingleton<WorkoutLogg.PageModels.WorkoutsPageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.WorkoutsPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.AddWorkoutPage>();
        builder.Services.AddSingleton<WorkoutLogg.PageModels.LoggerPageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.LoggerPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.AddLogPage>();

        const string baseUrl = "https://localhost:5001";
        builder.Services.AddRefitClient<IAuthApi>()
              .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
			  .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAuthRefreshApi>()
				.ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl));

        builder.Services.AddRefitClient<WorkoutLogg.Services.IWorkoutsApi>()
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl));

        builder.Services.AddSingleton<WorkoutLogg.Services.WorkoutSyncService>();

        builder.Services.AddSingleton(_ =>
        {
            // На Android эмуляторе хост машины — 10.0.2.2
            var address = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:5001"
                : "https://localhost:5001";
            return new ExercisesGrpcClient(address);
        });

        builder.Services.AddFluentValidation();


        return builder.Build();
	}
}
