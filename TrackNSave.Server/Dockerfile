FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Установка всех необходимых зависимостей
RUN apt-get update && \
    apt-get install -y --allow-unauthenticated \
    libgdiplus \
    xvfb \
    libfontconfig \
    wkhtmltopdf \
    libc6-dev \
    openssl \
    libssl-dev \
    libxrender1 \
    libfontconfig1 \
    libxext-dev

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_20.x | bash && \
    apt-get install -y nodejs

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копирование файлов библиотек (должны находиться в корне проекта)
COPY ["libwkhtmltox.so", "libwkhtmltox.dylib", "libwkhtmltox.dll", "./"]
COPY ["TrackNSave.Server/TrackNSave.Server.csproj", "TrackNSave.Server/"]
COPY ["tracknsave.client/tracknsave.client.esproj", "tracknsave.client/"]

RUN dotnet restore "./TrackNSave.Server/TrackNSave.Server.csproj"
COPY . .
WORKDIR "/src/TrackNSave.Server"
RUN dotnet build "./TrackNSave.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TrackNSave.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .

# Копирование библиотек в выходную директорию
COPY --from=build /src/libwkhtmltox.so /app/
COPY --from=build /src/libwkhtmltox.dylib /app/
COPY --from=build /src/libwkhtmltox.dll /app/

# Настройка переменных окружения
ENV LD_LIBRARY_PATH="/app:$LD_LIBRARY_PATH"
RUN ldconfig

ENTRYPOINT ["dotnet", "TrackNSave.Server.dll"]