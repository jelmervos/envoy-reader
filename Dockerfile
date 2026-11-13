# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-aot AS build
WORKDIR /source

RUN apt-get update && apt-get install -y tzdata

COPY --link ["EnvoyReader2/", "EnvoyReader2/"]
COPY --link ["nuget.config", "./"]
COPY --link ["Packages/", "./Packages/"]

RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    dotnet publish EnvoyReader2/EnvoyReader2.csproj \
        -r linux-x64 \
        -o /app \
        -c Release \
        && rm /app/*.dbg

RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled
WORKDIR /app

COPY --from=build /usr/share/zoneinfo /usr/share/zoneinfo

COPY --link --from=build --chown=$APP_UID:$APP_UID /app .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
USER $APP_UID
VOLUME /app/data
ENTRYPOINT ["./EnvoyReader2"]