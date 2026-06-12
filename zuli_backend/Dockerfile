# Backend .NET 10 Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copiar archivos de solución y proyecto primero para aprovechar cache
COPY zuli_backend/zuli_backend.slnx ./
COPY zuli_backend/zuli_backend/zuli_backend.csproj zuli_backend/
COPY zuli_backend/zuli_buisiness/zuli_Business.csproj zuli_buisiness/
COPY zuli_backend/zuli_Data/zuli_Data.csproj zuli_Data/
COPY zuli_backend/zuli_Repository/zuli_Repository.csproj zuli_Repository/
COPY zuli_backend/Tests/Tests.csproj Tests/

# Restaurar dependencias
RUN dotnet restore zuli_backend/zuli_backend.csproj

# Copiar todo el código fuente
COPY zuli_backend/ .

# Compilar y publicar
RUN dotnet publish zuli_backend/zuli_backend.csproj -c Release -o /app/publish --no-restore

# Stage de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Crear usuario no-root para seguridad
RUN useradd -m -s /bin/bash appuser && chown -R appuser /app
USER appuser

# Copiar publicación
COPY --from=build /app/publish .

# Exponer puerto
EXPOSE 8080

ENTRYPOINT ["dotnet", "zuli_backend.dll"]
