#!/bin/sh
set -e

# Запускаем Nginx с временной конфигурацией
nginx

# Создаем директорию для certbot
mkdir -p /var/www/certbot

# Получаем сертификат
certbot certonly --webroot -w /var/www/certbot -d tracknsave.ru -d www.tracknsave.ru --email ermak153@yandex.ru --agree-tos --no-eff-email --non-interactive

# Останавливаем Nginx
nginx -s stop

# Ждем остановки Nginx
sleep 2

# Заменяем конфигурацию на финальную с HTTPS
cp /etc/nginx/conf.d/default.conf.ssl /etc/nginx/conf.d/default.conf

# Настройка cron для обновления сертификата
echo "0 12 * * * certbot renew --quiet && nginx -s reload" > /etc/crontabs/root
crond

# Запускаем Nginx с новой конфигурацией
exec nginx -g "daemon off;"
