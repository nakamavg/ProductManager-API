FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["ProductosAPI.Core.csproj", "./"]
RUN dotnet restore

# Copiar todo el código fuente y compilar
COPY . .
RUN dotnet build -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Crear imagen final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductosAPI.Core.dll"]