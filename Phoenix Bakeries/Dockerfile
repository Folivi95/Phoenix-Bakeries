FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Phoenix Bakeries/Phoenix Bakeries.csproj", "Phoenix Bakeries/"]
RUN dotnet restore "Phoenix Bakeries/Phoenix Bakeries.csproj"
COPY . .
WORKDIR "/src/Phoenix Bakeries"
RUN dotnet build "Phoenix Bakeries.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Phoenix Bakeries.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Phoenix Bakeries.dll"]