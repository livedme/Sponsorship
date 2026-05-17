# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY Sponsorship.slnx ./
COPY src/Sponsorship.BlazorUI/Sponsorship.BlazorUI.csproj src/Sponsorship.BlazorUI/

RUN dotnet restore src/Sponsorship.BlazorUI/Sponsorship.BlazorUI.csproj

# Copy all source files and publish
COPY . .
RUN dotnet publish src/Sponsorship.BlazorUI/Sponsorship.BlazorUI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system --gid 1001 appgroup \
 && adduser  --system --uid 1001 --ingroup appgroup appuser

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app

USER appuser

ENV ASPNETCORE_URLS=http://+:7001
EXPOSE 7001

ENTRYPOINT ["dotnet", "Sponsorship.BlazorUI.dll"]
