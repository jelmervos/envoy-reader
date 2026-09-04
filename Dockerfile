FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG RUNTIME=linux-musl-x64
WORKDIR /src

COPY ["EnvoyReader2/EnvoyReader2.csproj", "EnvoyReader2/"]
COPY ["nuget.config", "./"]
COPY ["Packages/", "./Packages/"]

RUN dotnet restore "./EnvoyReader2/EnvoyReader2.csproj" -r "$RUNTIME" /p:PublishReadyToRun=false

COPY . .
RUN dotnet publish "./EnvoyReader2/EnvoyReader2.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -r "$RUNTIME" \
    --self-contained false \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs tzdata

WORKDIR /app
USER app
VOLUME /app/data

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EnvoyReader2.dll"]
