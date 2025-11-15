# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["cl-backend/cl-backend.csproj", "cl-backend/"]
RUN dotnet restore "cl-backend/cl-backend.csproj"

# Копируем все файлы и собираем проект
COPY . .
WORKDIR "/src/cl-backend"
RUN dotnet build "cl-backend.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "cl-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Создаем директорию для загрузок
RUN mkdir -p /app/uploads

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cl-backend.dll"]
