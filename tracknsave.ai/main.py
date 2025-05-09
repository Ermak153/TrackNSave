from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import torch
from transformers import XLMRobertaTokenizer, XLMRobertaForSequenceClassification
import os
import pickle
import gdown
from filelock import FileLock

app = FastAPI(title="Product Classifier")

CATEGORIES = [
    "Общественный транспорт", "Аптечные товары", "Дом и ремонт", "Животные", 
    "Развлечения", "Красота", "Музыка", "Образование", "Одежда и обувь", 
    "Спорттовары", "Топливо", "Цветы", "Продукты питания", "Услуги", 
    "Автотовары", "Электроника", "Канцелярия", "Табачная продукция", 
    "Ювелирные изделия и аксессуары", "Различные товары"
]

class ModelLoader:
    def __init__(self):
        self.lock_path = "/tmp/model.lock"
        self.model = None
        self.tokenizer = None
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.model_file_id = os.getenv("MODEL_FILE_ID", "1utoun62SV_2MAaRyuLDEfH3dPzi-CvqQ")
        self.model_dir = os.getenv("MODEL_PATH", "./models/xlm-roberta-large")
        self.model_path = os.path.join(self.model_dir, "model.safetensors")

    def _download_model(self):
        os.makedirs(self.model_dir, exist_ok=True)
        if os.path.exists(self.model_path):
            return True
            
        url = f'https://drive.google.com/uc?id={self.model_file_id}'

        with FileLock(self.lock_path + ".lock"):
            if os.path.exists(self.model_path):
                return True
            
        try:
            gdown.download(url, self.model_path, quiet=False)
            if os.path.getsize(self.model_path) < 1024*1024:
                os.remove(self.model_path)
                raise ValueError("Downloaded file is too small - likely error")
            return True
        except Exception as e:
            if os.path.exists(self.model_path):
                os.remove(self.model_path)
            raise RuntimeError(f"Model download failed: {str(e)}")

    def load_model(self):
        try:
            if not os.path.exists(self.model_path):
                self._download_model()

            self.tokenizer = XLMRobertaTokenizer.from_pretrained(self.model_dir)
            self.model = XLMRobertaForSequenceClassification.from_pretrained(self.model_dir)
            self.model.to(self.device)
            self.model.eval()

            label_encoder_path = os.path.join(self.model_dir, "label_encoder.pkl")
            if os.path.exists(label_encoder_path):
                with open(label_encoder_path, 'rb') as f:
                    self.label_encoder = pickle.load(f)
            else:
                self.label_encoder = None

            return True
        except Exception as e:
            raise RuntimeError(f"Model loading error: {str(e)}")

model_loader = ModelLoader()

class ProductItems(BaseModel):
    items: List[str]

class ClassificationResult(BaseModel):
    product: str
    category: str
    confidence: float

@app.on_event("startup")
async def initialize_model():
    try:
        if not model_loader.load_model():
            raise HTTPException(status_code=500, detail="Model initialization failed")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/classify_batch", response_model=List[ClassificationResult])
async def batch_classification(products: ProductItems):
    if model_loader.model is None:
        raise HTTPException(status_code=503, detail="Model not ready")

    results = []
    for item in products.items:
        inputs = model_loader.tokenizer(
            item,
            return_tensors="pt",
            padding="max_length",
            truncation=True,
            max_length=128
        ).to(model_loader.device)

        with torch.no_grad():
            outputs = model_loader.model(**inputs)
        
        probs = torch.nn.functional.softmax(outputs.logits, dim=-1)
        conf, class_id = torch.max(probs, dim=-1)

        if model_loader.label_encoder:
            category = model_loader.label_encoder.inverse_transform([class_id.item()])[0]
        else:
            category = CATEGORIES[class_id] if class_id < len(CATEGORIES) else CATEGORIES[-1]

        results.append(ClassificationResult(
            product=item,
            category=category,
            confidence=conf.item()
        ))

    return results

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)