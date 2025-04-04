FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY /WebApplication1/bin/Release/net8.0/publish /app
VOLUME /app/config
VOLUME /app/logs
ENTRYPOINT ["dotnet","WebApplication1.dll"]