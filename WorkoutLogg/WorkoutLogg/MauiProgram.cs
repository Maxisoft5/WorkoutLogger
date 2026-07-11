using AKSoftware.Localization.MultiLanguages;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Infrastructure.Api;
using Modules.Users.Infrastructure.Authorization;
using Refit;
using Syncfusion.Maui.Toolkit.Hosting;
using WorkoutLogg.Database;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;
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

        // Load appsettings.json from app bundle (Resources/Raw/appsettings.json)
        var useLocalhost    = false;
        var testMode        = false;
        var vpsUrl          = "https://202.148.55.20:5001";
        var localUrl        = "https://localhost:5001";
        var localAndroidUrl = "https://10.0.2.2:5001";

        try
        {
            var cfgTask = FileSystem.OpenAppPackageFileAsync("appsettings.json");
            cfgTask.Wait(TimeSpan.FromSeconds(3));
            if (cfgTask.IsCompletedSuccessfully)
            {
                using var stream = cfgTask.Result;
                using var doc = System.Text.Json.JsonDocument.Parse(stream);
                var root = doc.RootElement;

                if (root.TryGetProperty("UseLocalhost", out var useLoc))
                    useLocalhost = useLoc.GetBoolean();

                if (root.TryGetProperty("TestMode", out var tm))
                    testMode = tm.GetBoolean();

                if (root.TryGetProperty("Api", out var api))
                {
                    if (api.TryGetProperty("VpsUrl", out var vps)) vpsUrl = vps.GetString() ?? vpsUrl;
                    if (api.TryGetProperty("LocalUrl", out var loc)) localUrl = loc.GetString() ?? localUrl;
                    if (api.TryGetProperty("LocalAndroidUrl", out var and)) localAndroidUrl = and.GetString() ?? localAndroidUrl;
                }
            }
        }
        catch { /* конфиг не загружен — используем defaults */ }

        builder.Services.AddSingleton(new WorkoutLogg.Services.AppConfiguration
        {
            UseLocalhost = useLocalhost,
            TestMode     = testMode,
        });

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
        builder.Services.AddTransient<WorkoutLogg.Pages.EditBodyStatsPage>();
        builder.Services.AddSingleton<WorkoutLogg.PageModels.DashboardPageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.DashboardPage>();
        builder.Services.AddSingleton<WorkoutLogg.Services.UserProfileService>();
        builder.Services.AddSingleton<WorkoutLogg.PageModels.ProfilePageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.ProfilePage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.StandardsPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.PremiumPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.PremiumComparePage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.PaymentPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.AiCoachPage>();
        // Trainer marketplace (student side): PageModel is a singleton so the selected
        // trainer can be shared between the list and the detail page without query serialisation.
        builder.Services.AddSingleton<WorkoutLogg.PageModels.TrainersPageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.TrainersPage>();
        builder.Services.AddTransient<WorkoutLogg.Pages.TrainerDetailPage>();
        builder.Services.AddSingleton<WorkoutLogg.PageModels.WalletPageModel>();
        builder.Services.AddTransient<WorkoutLogg.Pages.WalletPage>();
        builder.Services.AddTransient<AppShell>();

        var baseUrl = useLocalhost
            ? (DeviceInfo.Platform == DevicePlatform.Android ? localAndroidUrl : localUrl)
            : vpsUrl;

#if DEBUG
        static HttpMessageHandler DevHandler() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };
#endif

        builder.Services.AddRefitClient<IAuthApi>()
              .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
              .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
              .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IAuthRefreshApi>()
              .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
              .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
              ;

        builder.Services.AddRefitClient<WorkoutLogg.Services.IWorkoutsApi>()
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
            ;

        builder.Services.AddRefitClient<WorkoutLogg.Services.ISubscriptionsApi>()
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
            ;

        builder.Services.AddRefitClient<WorkoutLogg.Services.IAiCoachApi>()
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
            ;

        // Trainers API serialises enums as strings (server uses JsonStringEnumConverter),
        // so this client is configured with the same converter to round-trip them by name.
        var trainersRefitSettings = new RefitSettings(
            new SystemTextJsonContentSerializer(
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                }));

        builder.Services.AddRefitClient<WorkoutLogg.Services.ITrainersApi>(trainersRefitSettings)
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
            ;

        builder.Services.AddRefitClient<WorkoutLogg.Services.IWalletApi>(trainersRefitSettings)
            .ConfigureHttpClient(b => b.BaseAddress = new Uri(baseUrl))
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(DevHandler)
#endif
            ;

        builder.Services.AddSingleton<WorkoutLogg.Services.WorkoutSyncService>();

        builder.Services.AddSingleton(_ =>
        {
            // gRPC работает по HTTP/1.1 на отдельном порту без TLS
            var grpcUrl = useLocalhost
                ? (DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000")
                : "http://202.148.55.20:5000";
            return new ExercisesGrpcClient(grpcUrl);
        });

        builder.Services.AddFluentValidation();


        // Localization — scans embedded *.yml resources in the app assembly
        builder.Services.AddSingleton<ILanguageContainerService>(_ =>
        {
            var keysProvider = new EmbeddedResourceKeysProvider(
                typeof(App).Assembly, "Resources.Languages");
            return new LanguageContainerInAssembly(keysProvider);
        });
        builder.Services.AddSingleton<LanguageService>();

        var mauiApp = builder.Build();

        // Initialise static Loc accessor and apply saved/detected language
        var langService = mauiApp.Services.GetRequiredService<LanguageService>();
        langService.ApplyPreferred();

        return mauiApp;
	}
}
