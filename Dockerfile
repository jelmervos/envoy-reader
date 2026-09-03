# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["EnvoyReader2/EnvoyReader2.csproj", "EnvoyReader2/"]
COPY ["nuget.config", "./"]
COPY ["Packages/", "./Packages/"]

RUN dotnet restore "EnvoyReader2/EnvoyReader2.csproj" -r linux-musl-x64

COPY . .
RUN dotnet publish "EnvoyReader2/EnvoyReader2.csproj" \
    -c Release \
    -r linux-musl-x64 \
    --self-contained true \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime (Alpine)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

RUN mkdir -p /app/data && chown -R app:app /app/data
USER app

ENTRYPOINT ["./EnvoyReader2"]