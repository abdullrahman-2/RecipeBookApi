# Use .NET 8 SDK as build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy only the csproj to restore packages
COPY RecipeBookaApi/*.csproj ./RecipeBookaApi/
RUN dotnet restore RecipeBookaApi/RecipeBookaApi.csproj

# Copy the entire project
COPY . .

# Build the app
RUN dotnet publish RecipeBookaApi/RecipeBookaApi.csproj -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "RecipeBookaApi.dll"]
