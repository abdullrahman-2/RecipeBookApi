# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy .csproj and restore as distinct layers
COPY RecipeBookaApi/*.csproj ./RecipeBookaApi/
RUN dotnet restore ./RecipeBookaApi/RecipeBookaApi.csproj

# Copy the rest of the code
COPY . .

# Build the project
RUN dotnet publish ./RecipeBookaApi/RecipeBookaApi.csproj -c Release -o out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the port
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "RecipeBookaApi.dll"]
