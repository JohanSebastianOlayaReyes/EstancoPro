# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Web/Web.csproj", "Web/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Entity/Entity.csproj", "Entity/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]

RUN dotnet restore "Web/Web.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar el proyecto
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

# Etapa 2: Publish
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar herramientas de espera (para SQL Server)
RUN apt-get update && apt-get install -y wget && \
    wget -qO /usr/local/bin/wait-for https://raw.githubusercontent.com/eficode/wait-for/v2.2.3/wait-for && \
    chmod +x /usr/local/bin/wait-for

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Web.dll"]
