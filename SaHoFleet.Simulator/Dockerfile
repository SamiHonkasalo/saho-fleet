FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SaHoFleet.Simulator/SaHoFleet.Simulator.csproj", "SaHoFleet.Simulator/"]
RUN dotnet restore "SaHoFleet.Simulator/SaHoFleet.Simulator.csproj"
COPY . .
WORKDIR "/src/SaHoFleet.Simulator"
RUN dotnet build "SaHoFleet.Simulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SaHoFleet.Simulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaHoFleet.Simulator.dll"]
