FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# See for all possible platforms
# https://github.com/containerd/containerd/blob/master/platforms/platforms.go#L17
ARG TARGETARCH

ARG VERSIONSUFFIX="docker"

WORKDIR /source

# Copy csproj and restore.
COPY src/Impostor.Server/Impostor.Server.csproj ./src/Impostor.Server/Impostor.Server.csproj
COPY src/Impostor.Api/Impostor.Api.csproj ./src/Impostor.Api/Impostor.Api.csproj
COPY src/Impostor.Hazel/Hazel/Hazel.csproj ./src/Impostor.Hazel/Hazel/Hazel.csproj

RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server/Impostor.Server.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Api/Impostor.Api.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Hazel/Hazel/Hazel.csproj

# Copy everything else.
COPY src/. ./src/
RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  [[ $VERSIONSUFFIX = "none" ]] && VERSIONSUFFIX=; \
  dotnet publish -c release -o /app -r "$NETCORE_PLATFORM" -p:VersionSuffix="$VERSIONSUFFIX" --no-restore ./src/Impostor.Server/Impostor.Server.csproj

# Final image.
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 22023/udp
ENTRYPOINT ["./Impostor.Server"]