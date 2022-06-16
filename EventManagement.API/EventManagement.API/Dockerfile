#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EventManagement.API/EventManagement.API.csproj", "EventManagement.API/"]
COPY ["EventManagement.Application/EventManagement.Application.csproj", "EventManagement.Application/"]
COPY ["EventManagement.Domain/EventManagement.Domain.csproj", "EventManagement.Domain/"]
COPY ["EventManagement.Infrastructure/EventManagement.Infrastructure.csproj", "EventManagement.Infrastructure/"]
RUN dotnet restore "EventManagement.API/EventManagement.API.csproj"
COPY . .
WORKDIR "/src/EventManagement.API"
RUN dotnet build "EventManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventManagement.API.dll"]
