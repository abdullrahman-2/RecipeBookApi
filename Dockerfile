# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy only the .csproj and restore
COPY RecipeBookaApi/*.csproj ./RecipeBookaApi/
RUN dotnet restore RecipeBookaApi/RecipeBookaApi.csproj

# Copy the rest of the project files
COPY . .

# Build the project
RUN dotnet publish RecipeBookaApi/RecipeBookaApi.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "RecipeBookaApi.dll"]
