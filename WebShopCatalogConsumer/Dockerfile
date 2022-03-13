#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebShopCatalogConsumer/WebShopCatalogConsumer.csproj", "WebShopCatalogConsumer/"]
RUN dotnet restore "WebShopCatalogConsumer/WebShopCatalogConsumer.csproj"
COPY . .
WORKDIR "/src/WebShopCatalogConsumer"
RUN dotnet build "WebShopCatalogConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebShopCatalogConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShopCatalogConsumer.dll"]
