FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# Copy csproj and restore.
COPY src/Impostor.Server/Impostor.Server.csproj ./src/Impostor.Server/Impostor.Server.csproj
COPY src/Impostor.Shared/Impostor.Shared.csproj ./src/Impostor.Shared/Impostor.Shared.csproj
COPY submodules/Hazel-Networking/Hazel/Hazel.csproj ./submodules/Hazel-Networking/Hazel/Hazel.csproj

RUN dotnet restore -r linux-musl-x64 ./src/Impostor.Server/Impostor.Server.csproj && \
    dotnet restore -r linux-musl-x64 ./src/Impostor.Shared/Impostor.Shared.csproj && \
    dotnet restore -r linux-musl-x64 ./submodules/Hazel-Networking/Hazel/Hazel.csproj

# Copy everything else.
COPY submodules/. ./submodules/
COPY src/. ./src/
RUN dotnet publish -c release -o /app -r linux-musl-x64 --no-restore ./src/Impostor.Server/Impostor.Server.csproj

# Final image.
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine
RUN echo 'http://dl-cdn.alpinelinux.org/alpine/v3.8/main' >> /etc/apk/repositories && \
    apk update --no-cache && \
    apk add --no-cache bash libc6-compat=1.1.19-r11
WORKDIR /app
COPY --from=build /app ./
EXPOSE 22023/udp
ENTRYPOINT ["./Impostor.Server"]