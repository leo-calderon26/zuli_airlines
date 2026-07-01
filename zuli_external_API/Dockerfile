# External API .NET 10 Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY zuli_external_API/zuli_external_API.slnx ./
COPY zuli_external_API/zuli_backend/zuli_external_API.csproj zuli_backend/
COPY zuli_external_API/zuli_buisiness/zuli_Business.csproj zuli_buisiness/
COPY zuli_external_API/zuli_Data/zuli_Data.csproj zuli_Data/
COPY zuli_external_API/zuli_Repository/zuli_Repository.csproj zuli_Repository/
COPY zuli_external_API/Tests/Tests.csproj Tests/

RUN dotnet restore zuli_backend/zuli_external_API.csproj

COPY zuli_external_API/ .

RUN dotnet publish zuli_backend/zuli_external_API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

RUN useradd -m -s /bin/bash appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 7064

ENTRYPOINT ["dotnet", "zuli_external_API.dll"]
