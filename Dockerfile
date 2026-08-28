FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Giromon.Api/Giromon.Api.csproj", "src/Giromon.Api/"]
COPY ["src/Giromon.Application/Giromon.Application.csproj", "src/Giromon.Application/"]
COPY ["src/Giromon.Domain/Giromon.Domain.csproj", "src/Giromon.Domain/"]
COPY ["src/Giromon.Infrastructure/Giromon.Infrastructure.csproj", "src/Giromon.Infrastructure/"]
RUN dotnet restore "src/Giromon.Api/Giromon.Api.csproj"

COPY . .
RUN dotnet publish "src/Giromon.Api/Giromon.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Giromon.Api.dll"]
