FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore as distinct layers to speed up docker builds.
COPY site_manga_home.csproj ./
RUN dotnet restore site_manga_home.csproj

COPY . .
RUN dotnet publish site_manga_home.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "site_manga_home.dll"]
