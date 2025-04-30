from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List, Optional
import torch
from transformers import XLMRobertaTokenizer, XLMRobertaForSequenceClassification
import os
import pickle

app = FastAPI(title="Product Classifier", description="API for classifying products by category")

CATEGORIES = [
    "Общественный транспорт", "Аптечные товары", "Дом и ремонт", "Животные", 
    "Развлечения", "Красота", "Музыка", "Образование", "Одежда и обувь", 
    "Спорттовары", "Топливо", "Цветы", "Продукты питания", "Услуги", 
    "Автотовары", "Электроника", "Канцелярия", "Табачная продукция", 
    "Ювелирные изделия и аксессуары", "Различные товары"
]

class ModelLoader:
    def __init__(self):
        self.model = None
        self.tokenizer = None
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.model_parts = [
            "model.safetensors_part_00",
            "model.safetensors_part_01"
        ]
    
    def _merge_model_parts(self, model_dir: str) -> Optional[str]:
        """Combines the parts of the model into a single file and returns the path to it"""
        merged_path = os.path.join(model_dir, "model.safetensors")
        
        if os.path.exists(merged_path):
            return merged_path
            
        for part in self.model_parts:
            if not os.path.exists(os.path.join(model_dir, part)):
                print(f"Missing model part: {part}")
                return None
                
        try:
            with open(merged_path, 'wb') as outfile:
                for part in self.model_parts:
                    part_path = os.path.join(model_dir, part)
                    with open(part_path, 'rb') as infile:
                        outfile.write(infile.read())
            print(f"Model parts successfully merged to {merged_path}")
            return merged_path
        except Exception as e:
            print(f"Error merging model parts: {str(e)}")
            return None
    
    def load_model(self):
        model_path = os.getenv("MODEL_PATH", "./models/xlm-roberta-large")
        
        try:
            self.tokenizer = XLMRobertaTokenizer.from_pretrained(model_path)
            
            merged_model_path = self._merge_model_parts(model_path)
            if not merged_model_path:
                raise RuntimeError("Could not merge model parts")

            self.model = XLMRobertaForSequenceClassification.from_pretrained(model_path)
            
            self.model.to(self.device)
            self.model.eval()
            
            label_encoder_path = os.path.join(model_path, "label_encoder.pkl")
            if os.path.exists(label_encoder_path):
                with open(label_encoder_path, 'rb') as f:
                    self.label_encoder = pickle.load(f)
                print(f"Label encoder is loaded: {list(self.label_encoder.classes_)}")
            else:
                print("Label encoder not found, we use predefined categories")
                self.label_encoder = None
            
            print(f"The model has been successfully uploaded to the device: {self.device}")
            return True
        except Exception as e:
            print(f"Error loading the model: {str(e)}")
            return False

model_loader = ModelLoader()

class ProductItem(BaseModel):
    name: str

class ProductItems(BaseModel):
    items: List[str]

class ClassificationResult(BaseModel):
    product: str
    category: str
    confidence: float

@app.on_event("startup")
async def startup_event():
    success = model_loader.load_model()
    if not success:
        print("Couldn't load the model. The server may not be working correctly")

@app.post("/classify_batch", response_model=List[ClassificationResult])
async def classify_products(products: ProductItems):
    if model_loader.model is None or model_loader.tokenizer is None:
        raise HTTPException(status_code=500, detail="The model is not loaded")
    
    results = [predict_category(item) for item in products.items]
    return results

def predict_category(product_name: str) -> ClassificationResult:
    """Product category prediction using the loaded model"""
    with torch.no_grad():
        inputs = model_loader.tokenizer(
            product_name,
            return_tensors="pt",
            padding="max_length",
            truncation=True,
            max_length=128
        ).to(model_loader.device)
        
        outputs = model_loader.model(**inputs)
        logits = outputs.logits
        
        probabilities = torch.nn.functional.softmax(logits, dim=-1)
        
        predicted_class_id = torch.argmax(probabilities, dim=-1).item()
        confidence = probabilities[0][predicted_class_id].item()
        
        if hasattr(model_loader, 'label_encoder') and model_loader.label_encoder is not None:
            predicted_category = model_loader.label_encoder.inverse_transform([predicted_class_id])[0]
        else:
            predicted_category = CATEGORIES[predicted_class_id] if predicted_class_id < len(CATEGORIES) else "Различные товары"
        
        return ClassificationResult(
            product=product_name,
            category=predicted_category,
            confidence=confidence
        )

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)