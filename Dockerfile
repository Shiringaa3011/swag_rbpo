# Версии среды совпадают с global.json и официальным каркасом S08.
FROM mcr.microsoft.com/dotnet/sdk:10.0.401@sha256:e70cdb7f80b0348f5cb85f19a8f670fca061f033d57eed12fa003d58b0e06317 AS build
WORKDIR /source

COPY global.json Directory.Build.props ./
COPY src/CampusPass.Api/CampusPass.Api.csproj src/CampusPass.Api/packages.lock.json src/CampusPass.Api/
RUN dotnet restore src/CampusPass.Api/CampusPass.Api.csproj --locked-mode

COPY src/CampusPass.Api/ src/CampusPass.Api/
RUN dotnet publish src/CampusPass.Api/CampusPass.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.13@sha256:8af87eba5e7cdafb1514d7388be287365da97c132509e84dfe252ffd552398ca AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "CampusPass.Api.dll"]
