FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# See for all possible platforms
# https://github.com/containerd/containerd/blob/master/platforms/platforms.go#L17
ARG TARGETARCH

ARG VERSIONSUFFIX="docker"

WORKDIR /source

# Copy csproj and restore.
COPY src/Impostor.Server/Impostor.Server.csproj ./src/Impostor.Server/Impostor.Server.csproj
COPY src/Impostor.Api/Impostor.Api.csproj ./src/Impostor.Api/Impostor.Api.csproj
COPY src/Directory.Build.props ./src/Directory.Build.props

RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server/Impostor.Server.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Api/Impostor.Api.csproj

# Copy everything else.
COPY src/. ./src/
RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  [ $VERSIONSUFFIX = "none" ] && VERSIONSUFFIX=; \
  dotnet publish -c release -o /app -r "$NETCORE_PLATFORM" -p:VersionSuffix="$VERSIONSUFFIX" --no-restore ./src/Impostor.Server/Impostor.Server.csproj

# Final image.
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

# Add Impostor.Http as a default built-in plugin
ADD https://github.com/Impostor/Impostor.Http/releases/download/v0.5.0/Impostor.Http.dll /app/builtin-plugins/
# Make it listen to 0.0.0.0 to expose it to the outside world.
ENV IMPOSTOR_HTTP_HttpServer__ListenIp=0.0.0.0
# Override ASPNETCORE_URLS to stop warning.
ENV ASPNETCORE_URLS=
# Enable the built-in plugin folder. Use a high number to prevent conflicts with existing configurations
ENV IMPOSTOR_PluginLoader__Paths__76=/app/builtin-plugins

EXPOSE 22023/tcp 22023/udp
ENTRYPOINT ["./Impostor.Server"]
