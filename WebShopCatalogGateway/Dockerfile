#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebShopCatalogGateway/WebShopCatalogGateway.csproj", "WebShopCatalogGateway/"]
RUN dotnet restore "WebShopCatalogGateway/WebShopCatalogGateway.csproj"
COPY . .
WORKDIR "/src/WebShopCatalogGateway"
RUN dotnet build "WebShopCatalogGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebShopCatalogGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShopCatalogGateway.dll"]