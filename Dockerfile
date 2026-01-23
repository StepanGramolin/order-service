# OrderService Dockerfile
# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ./OrderService/OrderService.csproj
RUN dotnet publish ./OrderService/OrderService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OrderService.dll"]