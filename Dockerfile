FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS builder
WORKDIR /app-source
COPY src/ src/
COPY test/ test/
COPY *.sln .
RUN dotnet restore
RUN dotnet publish -c release -o /app-publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app-publish
COPY --from=builder ./app-publish /app-publish
CMD ["sh", "-c", "dotnet CTeleport.DistanceMeter.${PRJ}.dll"]
