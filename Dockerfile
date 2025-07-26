# استخدم SDK المناسب لـ .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# انسخ ملفات المشروع فقط لخطوة الـ restore لتحسين الـ caching
# افترض أن ملف المشروع موجود في مسار RecipeBookaApi/RecipeBookaApi.csproj
COPY ["RecipeBookaApi/RecipeBookaApi.csproj", "RecipeBookaApi/"]

# قم بعمل restore للـ dependencies
# تأكد من استخدام المسار الصحيح لملف الـ .csproj داخل الـ container
RUN dotnet restore "RecipeBookaApi/RecipeBookaApi.csproj"

# انسخ باقي ملفات المشروع
# هذا سيجعل الـ build layer تتغير فقط عند تغيير ملفات السورس
COPY . .

# بناء التطبيق
# تأكد من استخدام المسار الصحيح لملف الـ .csproj
RUN dotnet build "RecipeBookaApi/RecipeBookaApi.csproj" -c Release -o /app/build

# نشر التطبيق
# تأكد من استخدام المسار الصحيح لملف الـ .csproj
# `--no-build` يمنع إعادة البناء لأننا قمنا بالبناء بالفعل في الخطوة السابقة
RUN dotnet publish "RecipeBookaApi/RecipeBookaApi.csproj" -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RecipeBookaApi.dll"]
