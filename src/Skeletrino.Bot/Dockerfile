FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY ["src/Skeletrino.Bot/Skeletrino.Bot.csproj", "Skeletrino.Bot/"]
RUN dotnet restore "src/Skeletrino.Bot/Skeletrino.Bot.csproj"
COPY . ./src
WORKDIR "/src/Skeletrino.Bot"
RUN dotnet publish "Skeletrino.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skeletrino.Bot.dll"]
