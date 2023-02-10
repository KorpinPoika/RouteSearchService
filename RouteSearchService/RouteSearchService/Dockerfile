FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RouteSearchService/RouteSearchService.csproj", "RouteSearchService/"]
RUN dotnet restore "RouteSearchService/RouteSearchService.csproj"
COPY . .
WORKDIR "/src/RouteSearchService"
RUN dotnet build "RouteSearchService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RouteSearchService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RouteSearchService.dll"]
