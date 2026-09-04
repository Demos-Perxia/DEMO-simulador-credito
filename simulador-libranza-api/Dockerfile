FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CuotaClara.Api/CuotaClara.Api.csproj", "src/CuotaClara.Api/"]
COPY ["src/CuotaClara.Application/CuotaClara.Application.csproj", "src/CuotaClara.Application/"]
COPY ["src/CuotaClara.Domain/CuotaClara.Domain.csproj", "src/CuotaClara.Domain/"]
COPY ["src/CuotaClara.Infrastructure/CuotaClara.Infrastructure.csproj", "src/CuotaClara.Infrastructure/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/CuotaClara.Api/CuotaClara.Api.csproj"
COPY . .
WORKDIR "/src/src/CuotaClara.Api"
RUN dotnet build "CuotaClara.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CuotaClara.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "CuotaClara.Api.dll"]
