FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# See for all possible platforms
# https://github.com/containerd/containerd/blob/master/platforms/platforms.go#L17
ARG TARGETARCH

ARG VERSIONSUFFIX="docker"

WORKDIR /source

# Copy csproj and restore.
COPY src/Impostor.Server/Impostor.Server.csproj ./src/Impostor.Server/Impostor.Server.csproj
COPY src/Impostor.Api.Innersloth.Generator/Impostor.Api.Innersloth.Generator.csproj ./src/Impostor.Api.Innersloth.Generator/Impostor.Api.Innersloth.Generator.csproj
COPY src/Impostor.Api/Impostor.Api.csproj ./src/Impostor.Api/Impostor.Api.csproj
COPY src/Directory.Build.props ./src/Directory.Build.props

RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server/Impostor.Server.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Api.Innersloth.Generator/Impostor.Api.Innersloth.Generator.csproj && \
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
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Override ASPNETCORE_URLS to stop warning.
ENV ASPNETCORE_URLS=

EXPOSE 22023/tcp 22023/udp
ENTRYPOINT ["./Impostor.Server"]
