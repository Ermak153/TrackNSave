# Используем CUDA-совместимый образ Python
FROM nvidia/cuda:12.1.1-runtime-ubuntu22.04

# Установка Python и необходимых системных зависимостей
RUN apt-get update && apt-get install -y \
    python3.11 \
    python3.11-dev \
    python3-pip \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Копируем только requirements.txt сначала
COPY tracknsave.ai/requirements.txt .

# Устанавливаем зависимости в отдельный слой
RUN pip3 install --no-cache-dir -r requirements.txt

# Копируем только необходимые файлы
COPY tracknsave.ai/main.py .
COPY tracknsave.ai/models/ ./models/

# Открываем порт
EXPOSE 8000

# Запускаем приложение
CMD ["python3", "-m", "uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"] 