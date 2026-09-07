# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-aot AS build
WORKDIR /source

COPY --link ["EnvoyReader2/", "EnvoyReader2/"]
COPY --link ["nuget.config", "./"]
COPY --link ["Packages/", "./Packages/"]

RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    dotnet publish EnvoyReader2/EnvoyReader2.csproj \
        -o /app \
        -c Release \
        && rm /app/*.dbg

RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled-extra
WORKDIR /app
COPY --link --from=build /app .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER $APP_UID
VOLUME /app/data
ENTRYPOINT ["./EnvoyReader2"]