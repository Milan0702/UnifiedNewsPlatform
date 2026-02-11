FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContentProcessor.csproj", "./"]
RUN dotnet restore "ContentProcessor.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ContentProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContentProcessor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentProcessor.dll"]
