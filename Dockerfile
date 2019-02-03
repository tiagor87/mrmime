FROM microsoft/dotnet:sdk AS builder
WORKDIR /source

COPY ./MrMime.sln .
COPY ./src/Api/MrMime.Api.csproj ./src/Api/MrMime.Api.csproj
COPY ./src/Core/MrMime.Core.csproj ./src/Core/MrMime.Core.csproj
COPY ./tests/Api/MrMime.Api.Tests.csproj ./tests/Api/MrMime.Api.Tests.csproj
COPY ./tests/Core/MrMime.Core.Tests.csproj ./tests/Core/MrMime.Core.Tests.csproj
RUN dotnet restore

COPY . .
RUN dotnet test ./tests/Core/MrMime.Core.Tests.csproj
RUN dotnet test ./tests/Api/MrMime.Api.Tests.csproj
 
RUN dotnet publish -c Release -o /app ./src/Api/MrMime.Api.csproj 

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

COPY --from=builder /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "./src/Api/MrMime.Api.csproj"]