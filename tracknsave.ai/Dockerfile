FROM nvidia/cuda:12.1.1-runtime-ubuntu22.04

RUN apt-get update && apt-get install -y \
    python3.11 \
    python3.11-dev \
    python3-pip \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY tracknsave.ai/requirements.txt .

RUN pip3 install --no-cache-dir -r requirements.txt

COPY tracknsave.ai/main.py .
COPY tracknsave.ai/models/ ./models/

EXPOSE 8000

CMD bash -c "python3 -c 'from main import model_loader; model_loader.load_model()' && uvicorn main:app --host 0.0.0.0 --port 8000 --workers 4"