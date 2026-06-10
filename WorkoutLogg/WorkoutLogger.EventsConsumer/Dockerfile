FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WorkoutLogger.EventsConsumer/WorkoutLogger.EventsConsumer.csproj", "WorkoutLogger.EventsConsumer/"]
COPY ["WorkoutLogg.ServiceDefaults/WorkoutLogg.ServiceDefaults.csproj", "WorkoutLogg.ServiceDefaults/"]
RUN dotnet restore "./WorkoutLogger.EventsConsumer/WorkoutLogger.EventsConsumer.csproj"
COPY . .
WORKDIR "/src/WorkoutLogger.EventsConsumer"
RUN dotnet build "./WorkoutLogger.EventsConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkoutLogger.EventsConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkoutLogger.EventsConsumer.dll"]
