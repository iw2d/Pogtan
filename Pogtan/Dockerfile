FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Pogtan/Pogtan.csproj", "Pogtan/"]
RUN dotnet restore "Pogtan/Pogtan.csproj"
COPY . .
WORKDIR "/src/Pogtan"
RUN dotnet build "Pogtan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pogtan.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pogtan.dll"]
