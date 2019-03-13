FROM microsoft/dotnet:sdk AS builder
WORKDIR /source

COPY ./MrMime.sln .
COPY ./src/Api/MrMime.Api.csproj ./src/Api/MrMime.Api.csproj
COPY ./src/Core/MrMime.Core.csproj ./src/Core/MrMime.Core.csproj
COPY ./tests/IntegrationTests/MrMime.IntegrationTests.csproj ./tests/IntegrationTests/MrMime.IntegrationTests.csproj
COPY ./tests/UnitTests/MrMime.UnitTests.csproj ./tests/UnitTests/MrMime.UnitTests.csproj
RUN dotnet restore

COPY . .
RUN dotnet test ./tests/UnitTests/MrMime.UnitTests.csproj
RUN dotnet test ./tests/IntegrationTests/MrMime.IntegrationTests.csproj
 
RUN dotnet publish -c Release -o /app ./src/Api/MrMime.Api.csproj 

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

COPY --from=builder /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT dotnet MrMime.Api.dll