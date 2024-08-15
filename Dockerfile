FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 5259

ENV ASPNETCORE_URLS=http://+:5259

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG configuration=Release
WORKDIR /src
COPY ["DotnetProject2025/DotnetProject2025.csproj", "DotnetProject2025/"]
RUN dotnet restore "DotnetProject2025\DotnetProject2025.csproj"
COPY . .
WORKDIR "/src/DotnetProject2025"
RUN dotnet build "DotnetProject2025.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "DotnetProject2025.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetProject2025.dll"]
