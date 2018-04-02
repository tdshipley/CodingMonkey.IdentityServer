FROM microsoft/aspnetcore-build:2.0.5 AS builder
# Stage 1
    WORKDIR /source

    # caches restore result by copying csproj file separately
    COPY ./src/CodingMonkey.IdentityServer/*.csproj .
    RUN dotnet restore

    # copies the rest of your code
    COPY ./src/CodingMonkey.IdentityServer/ .
    RUN dotnet publish --output /app/ --configuration Release

# Stage 2
    FROM microsoft/aspnetcore
    WORKDIR /app
    COPY --from=builder /app .
    CMD ASPNETCORE_URLS=http://*:$PORT dotnet CodingMonkey.IdentityServer.dll