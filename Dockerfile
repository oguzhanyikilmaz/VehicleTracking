FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Projeyi kopyala
COPY . .

# Bağımlılıkları restore et ve projeyi derle
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Çalışma zamanı imajı
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Sağlık kontrolü için healthcheck ekle
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Kullanılacak portlar
EXPOSE 7001  # TCP Server port
EXPOSE 80    # HTTP port

# Timezone ayarı
ENV TZ=Europe/Istanbul
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Yüksek güvenlik için non-root kullanıcı oluştur ve kullan
RUN adduser --disabled-password --gecos "" appuser
USER appuser

ENTRYPOINT ["dotnet", "VehicleTracking.API.dll"] 