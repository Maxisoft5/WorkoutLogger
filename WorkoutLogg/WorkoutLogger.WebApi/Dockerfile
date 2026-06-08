FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["WorkoutLogger.WebApi/WorkoutLogger.WebApi.csproj", "WorkoutLogger.WebApi/"]
COPY ["Modules.Common.Grpc.Contacts/Modules.Common.Grpc.Contacts.csproj", "Modules.Common.Grpc.Contacts/"]
COPY ["Modules.Common.Infrastructure/Modules.Common.Infrastructure.csproj", "Modules.Common.Infrastructure/"]
COPY ["Modules.Common.Domain/Modules.Common.Domain.csproj", "Modules.Common.Domain/"]
COPY ["Modules.Users.Domain/Modules.Users.Domain.csproj", "Modules.Users.Domain/"]
COPY ["Modules.Users.Infrastructure/Modules.Users.Infrastructure.csproj", "Modules.Users.Infrastructure/"]
COPY ["Modules.Users.DTO/Modules.Users.DTO.csproj", "Modules.Users.DTO/"]
COPY ["Moduels.Workouts.DTO/Moduels.Workouts.DTO.csproj", "Moduels.Workouts.DTO/"]
COPY ["WorkoutLogg.ServiceDefaults/WorkoutLogg.ServiceDefaults.csproj", "WorkoutLogg.ServiceDefaults/"]

RUN dotnet restore "./WorkoutLogger.WebApi/WorkoutLogger.WebApi.csproj"
COPY . .
WORKDIR "/src/WorkoutLogger.WebApi"
RUN dotnet build "./WorkoutLogger.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkoutLogger.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkoutLogger.WebApi.dll"]
