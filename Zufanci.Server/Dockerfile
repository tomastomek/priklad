#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Zufanci.Server/Zufanci.Server.csproj", "Zufanci.Server/"]
COPY ["Zufanci.Client/Zufanci.Client.csproj", "Zufanci.Client/"]
COPY ["Zufanci.Shared/Zufanci.Shared.csproj", "Zufanci.Shared/"]
RUN dotnet restore "./Zufanci.Server/Zufanci.Server.csproj"
COPY . .
WORKDIR "/src/Zufanci.Server"
RUN dotnet build "./Zufanci.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Zufanci.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zufanci.Server.dll"]