FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContentDeliveryService.csproj", "./"]
RUN dotnet restore "ContentDeliveryService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ContentDeliveryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContentDeliveryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentDeliveryService.dll"]
