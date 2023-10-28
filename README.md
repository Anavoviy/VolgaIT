# VolgaIT
!Приложение построено с помощью Docker-compose и, соответсвенно, самого Docker!
!Для его запуска необходим Docker Desktop (предпочтительно с Linux)!

ПОРЯДОК ЗАПУСКА ПРИЛОЖЕНИЯ:
1) Скачиваем архив и разархивируем его
2) В директории, где находится docker-compose.yml файл открываем консоль/терминал или PowerShell
3) Прописываем команду docker-compose build
4) Ждём сборки
5) Прописываем команду docker-compose up
6) Ждём поочерёдного поднятия PostgreSQL, WEB-API, PgAdmin

## URL: http://localhost:80/swagger/index.html   - Swagger
## URL: http://localhost:5050/browser/           - PgAdmin
