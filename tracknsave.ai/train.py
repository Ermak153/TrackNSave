import pandas as pd
import numpy as np
import torch
from torch.utils.data import Dataset, DataLoader
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.utils.class_weight import compute_class_weight
from sklearn.preprocessing import LabelEncoder
from transformers import XLMRobertaTokenizer, XLMRobertaForSequenceClassification
from transformers import get_linear_schedule_with_warmup
from torch.optim import AdamW
from torch.cuda.amp import autocast, GradScaler
from tqdm import tqdm
import matplotlib.pyplot as plt
import re
import os
import pickle
from multiprocessing import Pool, cpu_count

MODEL_NAME = 'xlm-roberta-large'
CSV_PATH = 'dataset.csv'
MAX_LEN = 128
BATCH_SIZE = 16
GRAD_ACCUM_STEPS = 2
EPOCHS = 5
SEED = 42
DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")

def parallel_apply(df, func, num_workers=cpu_count()):
    """Parallel processing of the DataFrame"""
    with Pool(num_workers) as pool:
        results = list(tqdm(pool.imap(func, df), total=len(df)))
    return pd.Series(results)

class ProductDataset(Dataset):
    def __init__(self, texts, labels, tokenizer, max_len=MAX_LEN):
        self.texts = texts
        self.labels = labels
        self.tokenizer = tokenizer
        self.max_len = max_len
        
    def __len__(self):
        return len(self.texts)
    
    def __getitem__(self, idx):
        text = str(self.texts[idx])
        label = self.labels[idx]
        
        encoding = self.tokenizer(
            text,
            add_special_tokens=True,
            max_length=self.max_len,
            padding='max_length',
            truncation=True,
            return_attention_mask=True,
            return_tensors='pt'
        )
        
        return {
            'input_ids': encoding['input_ids'].flatten(),
            'attention_mask': encoding['attention_mask'].flatten(),
            'labels': torch.tensor(label, dtype=torch.long)
        }

def preprocess_text(text):
    text = re.sub(r'[^\w\s]', ' ', str(text))
    text = re.sub(r'\s+', ' ', text)
    return text.strip().lower()

def train_model(model, train_dataloader, val_dataloader, device, epochs=EPOCHS, class_weights=None):
    optimizer_grouped_parameters = [
        {
            'params': [p for n, p in model.named_parameters() if 'bias' not in n and 'LayerNorm.weight' not in n],
            'weight_decay': 0.01,
            'lr': 2e-5
        },
        {
            'params': [p for n, p in model.named_parameters() if 'bias' in n or 'LayerNorm.weight' in n],
            'weight_decay': 0.0,
            'lr': 1e-5
        }
    ]
    
    optimizer = AdamW(optimizer_grouped_parameters, lr=2e-5, eps=1e-6)
    total_steps = len(train_dataloader) * epochs // GRAD_ACCUM_STEPS
    scheduler = get_linear_schedule_with_warmup(
        optimizer,
        num_warmup_steps=int(total_steps * 0.1),
        num_training_steps=total_steps
    )
    
    scaler = GradScaler()
    loss_fn = torch.nn.CrossEntropyLoss(weight=class_weights.to(device) if class_weights is not None else None)
    
    training_stats = []
    best_val_f1 = 0

    for epoch in range(epochs):
        print(f'\nEpoch {epoch + 1}/{epochs}')
        
        model.train()
        total_train_loss = 0
        all_preds = []
        all_labels = []
        optimizer.zero_grad()

        for step, batch in enumerate(tqdm(train_dataloader, desc="Training")):
            input_ids = batch['input_ids'].to(device, non_blocking=True)
            attention_mask = batch['attention_mask'].to(device, non_blocking=True)
            labels = batch['labels'].to(device, non_blocking=True)

            with autocast():
                outputs = model(input_ids=input_ids, attention_mask=attention_mask)
                loss = loss_fn(outputs.logits, labels) / GRAD_ACCUM_STEPS

            scaler.scale(loss).backward()

            if (step + 1) % GRAD_ACCUM_STEPS == 0:
                scaler.unscale_(optimizer)
                torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
                scaler.step(optimizer)
                scaler.update()
                scheduler.step()
                optimizer.zero_grad()

            total_train_loss += loss.item() * GRAD_ACCUM_STEPS
            all_preds.extend(torch.argmax(outputs.logits, dim=1).cpu().numpy())
            all_labels.extend(labels.cpu().numpy())

        avg_train_loss = total_train_loss / len(train_dataloader)
        train_report = classification_report(all_labels, all_preds, output_dict=True)
        
        model.eval()
        total_val_loss = 0
        val_preds = []
        val_labels = []

        with torch.inference_mode():
            for batch in tqdm(val_dataloader, desc="Validation"):
                input_ids = batch['input_ids'].to(device, non_blocking=True)
                attention_mask = batch['attention_mask'].to(device, non_blocking=True)
                labels = batch['labels'].to(device, non_blocking=True)

                with autocast():
                    outputs = model(input_ids=input_ids, attention_mask=attention_mask)
                    loss = loss_fn(outputs.logits, labels)

                total_val_loss += loss.item()
                val_preds.extend(torch.argmax(outputs.logits, dim=1).cpu().numpy())
                val_labels.extend(labels.cpu().numpy())

        avg_val_loss = total_val_loss / len(val_dataloader)
        val_report = classification_report(val_labels, val_preds, output_dict=True)

        print(f"Train Loss: {avg_train_loss:.4f} | Val Loss: {avg_val_loss:.4f}")
        print(f"Train F1: {train_report['macro avg']['f1-score']:.4f} | Val F1: {val_report['macro avg']['f1-score']:.4f}")

        if val_report['macro avg']['f1-score'] > best_val_f1:
            best_val_f1 = val_report['macro avg']['f1-score']
            torch.save(model.state_dict(), 'best_model.pt')
            print("The best model is saved")

        training_stats.append({
            'epoch': epoch + 1,
            'train_loss': avg_train_loss,
            'val_loss': avg_val_loss,
            'train_f1': train_report['macro avg']['f1-score'],
            'val_f1': val_report['macro avg']['f1-score']
        })
    
    return model, training_stats

def main():
    torch.manual_seed(SEED)
    np.random.seed(SEED)
    torch.backends.cudnn.benchmark = True
    
    print(f"The device used: {DEVICE}")
    if torch.cuda.is_available():
        print(f"GPU Memory: {torch.cuda.get_device_properties(0).total_memory / 1e9:.2f} GB")

    df = pd.read_csv(CSV_PATH, delimiter=';', engine='c')
    
    print("Text preprocessing...")
    df['cleaned_name'] = parallel_apply(df['product_name'], preprocess_text)

    le = LabelEncoder()
    df['category_encoded'] = le.fit_transform(df['category'])
    print(f"\nNumber of classes: {len(le.classes_)}")
    
    X_train, X_temp, y_train, y_temp = train_test_split(
        df['cleaned_name'],
        df['category_encoded'],
        test_size=0.3,
        random_state=SEED,
        stratify=df['category_encoded']
    )
    
    X_val, X_test, y_val, y_test = train_test_split(
        X_temp,
        y_temp,
        test_size=0.5,
        random_state=SEED,
        stratify=y_temp
    )

    class_weights = compute_class_weight(
        'balanced',
        classes=np.unique(y_train),
        y=y_train
    )
    class_weights = torch.tensor(class_weights, dtype=torch.float32)

    tokenizer = XLMRobertaTokenizer.from_pretrained(MODEL_NAME)
    model = XLMRobertaForSequenceClassification.from_pretrained(
        MODEL_NAME,
        num_labels=len(le.classes_),
        ignore_mismatched_sizes=True
    ).to(DEVICE)

    model.gradient_checkpointing_enable()

    train_dataset = ProductDataset(X_train.values, y_train.values, tokenizer)
    val_dataset = ProductDataset(X_val.values, y_val.values, tokenizer)
    test_dataset = ProductDataset(X_test.values, y_test.values, tokenizer)

    train_loader = DataLoader(
        train_dataset,
        batch_size=BATCH_SIZE,
        shuffle=True,
        num_workers=4,
        pin_memory=True
    )
    val_loader = DataLoader(
        val_dataset,
        batch_size=BATCH_SIZE*2,
        num_workers=4,
        pin_memory=True
    )
    test_loader = DataLoader(
        test_dataset,
        batch_size=BATCH_SIZE*2,
        num_workers=4,
        pin_memory=True
    )

    model, training_stats = train_model(
        model,
        train_loader,
        val_loader,
        DEVICE,
        epochs=EPOCHS,
        class_weights=class_weights
    )

    plot_training_results(training_stats)

    model.load_state_dict(torch.load('best_model.pt'))
    model.eval()
    
    all_preds = []
    all_labels = []
    with torch.inference_mode():
        for batch in tqdm(test_loader, desc="Testing"):
            input_ids = batch['input_ids'].to(DEVICE, non_blocking=True)
            attention_mask = batch['attention_mask'].to(DEVICE, non_blocking=True)
            labels = batch['labels'].to(DEVICE, non_blocking=True)
            
            outputs = model(input_ids=input_ids, attention_mask=attention_mask)
            all_preds.extend(torch.argmax(outputs.logits, dim=1).cpu().numpy())
            all_labels.extend(labels.cpu().numpy())

    print("\nFinal results:")
    print(classification_report(all_labels, all_preds, target_names=le.classes_))

    output_dir = './saved_model/'
    os.makedirs(output_dir, exist_ok=True)
    
    model.save_pretrained(output_dir)
    tokenizer.save_pretrained(output_dir)
    
    with open(os.path.join(output_dir, 'label_encoder.pkl'), 'wb') as f:
        pickle.dump(le, f)

def plot_training_results(stats):
    plt.figure(figsize=(12, 5))
    
    plt.subplot(1, 2, 1)
    plt.plot([s['epoch'] for s in stats], [s['train_loss'] for s in stats], 'b-o', label='Train')
    plt.plot([s['epoch'] for s in stats], [s['val_loss'] for s in stats], 'r-o', label='Validation')
    plt.title('Loss Dynamics')
    plt.xlabel('Epoch')
    plt.ylabel('Loss')
    plt.legend()
    plt.grid(True)
    
    plt.subplot(1, 2, 2)
    plt.plot([s['epoch'] for s in stats], [s['train_f1'] for s in stats], 'b-o', label='Train')
    plt.plot([s['epoch'] for s in stats], [s['val_f1'] for s in stats], 'r-o', label='Validation')
    plt.title('F1-Score Dynamics')
    plt.xlabel('Epoch')
    plt.ylabel('F1-Score')
    plt.legend()
    plt.grid(True)
    
    plt.tight_layout()
    plt.savefig('training_results.png')
    plt.show()

if __name__ == "__main__":
    main()