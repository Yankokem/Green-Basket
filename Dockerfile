FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Green Basket.csproj", "./"]
RUN dotnet restore "Green Basket.csproj"

# Copy remaining source code and publish
COPY . .
RUN dotnet publish "Green Basket.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
ENV ConnectionStrings__DefaultConnection="Data Source=GreenBasket_DRP.db"
EXPOSE 10000

ENTRYPOINT ["dotnet", "Green Basket.dll"]
