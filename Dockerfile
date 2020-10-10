FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# See for all possible platforms
# https://github.com/containerd/containerd/blob/master/platforms/platforms.go#L17
ARG TARGETARCH

WORKDIR /source

# Copy csproj and restore.
COPY src/Impostor.Server/Impostor.Server.csproj ./src/Impostor.Server/Impostor.Server.csproj
COPY src/Impostor.Server.Api/Impostor.Server.Api.csproj ./src/Impostor.Server.Api/Impostor.Server.Api.csproj
COPY src/Impostor.Server.Hazel/Impostor.Server.Hazel.csproj ./src/Impostor.Server.Hazel/Impostor.Server.Hazel.csproj
COPY src/Impostor.Shared/Impostor.Shared.csproj ./src/Impostor.Shared/Impostor.Shared.csproj
COPY submodules/Hazel-Networking/Hazel/Hazel.csproj ./submodules/Hazel-Networking/Hazel/Hazel.csproj

RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server/Impostor.Server.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server.Api/Impostor.Server.Api.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Server.Hazel/Impostor.Server.Hazel.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./src/Impostor.Shared/Impostor.Shared.csproj && \
  dotnet restore -r "$NETCORE_PLATFORM" ./submodules/Hazel-Networking/Hazel/Hazel.csproj

# Copy everything else.
COPY submodules/. ./submodules/
COPY src/. ./src/
RUN case "$TARGETARCH" in \
    amd64)  NETCORE_PLATFORM='linux-x64';; \
    arm64)  NETCORE_PLATFORM='linux-arm64';; \
    arm)    NETCORE_PLATFORM='linux-arm';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac && \
  dotnet publish -c release -o /app -r "$NETCORE_PLATFORM" --no-restore ./src/Impostor.Server/Impostor.Server.csproj

# Final image.
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/runtime-deps:5.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 22023/udp
ENTRYPOINT ["./Impostor.Server"]