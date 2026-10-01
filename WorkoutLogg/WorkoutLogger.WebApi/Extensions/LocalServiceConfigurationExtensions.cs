using DotNetEnv;
using DotNetEnv.Configuration;
using Npgsql;

namespace WorkoutLogger.WebApi.Extensions;

public static class LocalServiceConfigurationExtensions
{
    public static void AddLocalServiceConfiguration(this WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("UseLocalhost"))
        {
            return;
        }

        if (builder.Environment.IsDevelopment())
        {
            // Общий с Compose файл находится рядом с папкой проекта Web API.
            // Не меняем окружение процесса и не перекрываем настройки IDE / CLI.
            var envPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
            if (File.Exists(envPath))
            {
                var envConfiguration = new ConfigurationBuilder().AddDotNetEnv(envPath, LoadOptions.NoEnvVars());
                builder.Configuration.Sources.Insert(0, envConfiguration.Sources.Single());
            }
        }

        var configuration = builder.Configuration;
        var password = configuration["POSTGRES_PASSWORD"];
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Для UseLocalhost=true задайте POSTGRES_PASSWORD в окружении запуска или в .env рядом с docker-compose.yml (Development).");
        }

        var signingKey = configuration["JWT_SIGNING_KEY"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "Для UseLocalhost=true задайте JWT_SIGNING_KEY в окружении запуска или в .env рядом с docker-compose.yml (Development).");
        }

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 5432,
            Database = configuration["POSTGRES_DB"] ?? "workoutLogger",
            Username = configuration["POSTGRES_USER"] ?? "postgres",
            Password = password
        };

        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connectionString.ConnectionString,
            ["ConnectionStrings:Redis"] = "localhost:6379",
            ["Kafka:BootstrapServers"] = "localhost:9094",
            ["OpenSearch:Url"] = "http://localhost:9200",
            ["AuthConfiguration:Key"] = signingKey
        });
    }
}
