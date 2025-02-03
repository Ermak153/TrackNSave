# TrackNSave

## 🌍 Language / Язык  
<details>
  <summary>English</summary>

## 📥 Installation Guide

Follow these steps to install and run the application:

1. Clone the repository:
   ```sh
   git clone https://github.com/Ermak153/TrackNSave.git
   cd tracknsave
   ```

2. Create a `.env` file in the root directory and add the following variables:
   ```env
   POSTGRES_DB=database_name
   POSTGRES_USER=postgres_username
   POSTGRES_PASSWORD=database_password
   PGADMIN_EMAIL=your_email
   PGADMIN_PASSWORD=password_for_admin_panel
   Jwt__Key=your_super_long_and_secret_key
   ```

3. Navigate to `tracknsave.client` and install dependencies:
   ```sh
   cd tracknsave.client
   npm i
   ```

4. Install and start Docker Desktop. Ensure Docker Engine is running.

5. Open the project in Visual Studio.

6. Select `docker-compose` as the startup project.

7. Run the application and wait for the setup to complete (approximately 30 minutes).

---

## 🔗 Access to Services

After successfully starting the project, you can access the services at the following URLs:

- **pgAdmin** → [http://localhost:5050/](http://localhost:5050/)
- **Client (Frontend)** → [https://localhost:5173/](https://localhost:5173/)
- **Сlient (Frontend) from other devices in your Wi-Fi** → [https://192.168.31.131:5173/](https://192.168.31.131:5173/)
- **pg_exporter** → [http://localhost:9187/](http://localhost:9187/)

</details>

---

<details>
  <summary>Русский</summary>

## 📥 Руководство по установке

Следуйте этим шагам для установки и запуска приложения:

1. Склонируйте репозиторий:
   ```sh
   git clone https://github.com/Ermak153/TrackNSave.git
   cd tracknsave
   ```

2. Создайте файл `.env` в корне и добавьте в него следующие переменные:
   ```env
   POSTGRES_DB=database_name
   POSTGRES_USER=postgres_username
   POSTGRES_PASSWORD=database_password
   PGADMIN_EMAIL=your_email
   PGADMIN_PASSWORD=password_for_admin_panel
   Jwt__Key=your_super_long_and_secret_key
   ```

3. Перейдите в `tracknsave.client` и установите зависимости:
   ```sh
   cd tracknsave.client
   npm i
   ```

4. Установите и запустите Docker Desktop. Убедитесь, что Docker Engine работает.

5. Откройте проект в Visual Studio.

6. Выберите `docker-compose` в качестве запускаемого проекта.

7. Запустите проект и дождитесь завершения установки (примерно 30 минут).

---

## 🔗 Доступ к сервисам

После успешного запуска проекта доступ к сервисам осуществляется по следующим ссылкам:

- **pgAdmin** → [http://localhost:5050/](http://localhost:5050/)
- **Клиент (Frontend)** → [https://localhost:5173/](https://localhost:5173/)
- **Клиент (Frontend) с других устройств в вашем Wi-Fi** → [https://192.168.31.131:5173/](https://192.168.31.131:5173/)
- **pg_exporter** → [http://localhost:9187/](http://localhost:9187/)
</details>
