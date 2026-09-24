FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/core/RealtimeChat.Application/RealtimeChat.Application.csproj src/core/RealtimeChat.Application/
COPY src/core/RealtimeChat.Common/RealtimeChat.Common.csproj src/core/RealtimeChat.Common/
COPY src/core/RealtimeChat.Contracts/RealtimeChat.Contracts.csproj src/core/RealtimeChat.Contracts/
COPY src/core/RealtimeChat.Domain/RealtimeChat.Domain.csproj src/core/RealtimeChat.Domain/
COPY src/infrastructure/RealtimeChat.DependencyInjection/RealtimeChat.DependencyInjection.csproj src/infrastructure/RealtimeChat.DependencyInjection/
COPY src/infrastructure/RealtimeChat.Infrastructure/RealtimeChat.Infrastructure.csproj src/infrastructure/RealtimeChat.Infrastructure/
COPY src/infrastructure/RealtimeChat.Persistence/RealtimeChat.Persistence.csproj src/infrastructure/RealtimeChat.Persistence/
COPY src/presentation/webapi/RealtimeChat.Api/RealtimeChat.Api.csproj src/presentation/webapi/RealtimeChat.Api/

RUN dotnet restore src/presentation/webapi/RealtimeChat.Api/RealtimeChat.Api.csproj

COPY . .
RUN dotnet publish src/presentation/webapi/RealtimeChat.Api/RealtimeChat.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "RealtimeChat.Api.dll"]
