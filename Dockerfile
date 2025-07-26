# استخدم SDK المناسب لـ .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# انسخ كل الملفات
COPY . .

# حدد ملف الـ .csproj صح
RUN dotnet restore "./RecipeBookaApi.csproj"

# بناء التطبيق
RUN dotnet build "RecipeBookaApi.csproj" -c Release -o /app/build
RUN dotnet publish "RecipeBookaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RecipeBookaApi.dll"]
