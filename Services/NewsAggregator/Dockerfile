FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NewsAggregator.csproj", "./"]
RUN dotnet restore "NewsAggregator.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NewsAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NewsAggregator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NewsAggregator.dll"]
