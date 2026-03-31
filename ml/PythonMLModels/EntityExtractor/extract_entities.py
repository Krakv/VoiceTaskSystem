import json
import pandas as pd
import torch
from sklearn.model_selection import train_test_split
from datasets import Dataset
from transformers import (
    AutoTokenizer,
    AutoModelForTokenClassification,
    TrainingArguments,
    Trainer
)

# ===== CONFIG =====
MODEL_NAME = "ai-forever/ruRoberta-large"

# ===== CUDA =====
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# ===== 1. Данные =====
with open("ner_task_dataset.json", "r", encoding="utf-8") as f:
    raw = json.load(f)

df = pd.DataFrame(raw)

# Уникальные метки
unique_labels = sorted(set(l for sublist in df["labels"] for l in sublist))
label2id = {l: i for i, l in enumerate(unique_labels)}
id2label = {i: l for l, i in label2id.items()}

# ===== 2. Train / Val =====
train_df, val_df = train_test_split(df, test_size=0.1, random_state=42)

train_ds = Dataset.from_pandas(train_df.reset_index(drop=True))
val_ds   = Dataset.from_pandas(val_df.reset_index(drop=True))

# ===== 3. Токенизация =====
tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME,
    add_prefix_space=True)

def tokenize_and_align_labels(examples):
    tokenized_inputs = tokenizer(
        examples["tokens"],
        truncation=True,
        padding="max_length",
        max_length=64,
        is_split_into_words=True
    )

    labels = []
    for i, word_labels in enumerate(examples["labels"]):
        token_labels = []
        word_ids = tokenized_inputs.word_ids(batch_index=i)
        previous_word_idx = None

        for word_idx in word_ids:
            if word_idx is None:
                token_labels.append(-100)
            elif word_idx != previous_word_idx:
                token_labels.append(label2id[word_labels[word_idx]])
            else:
                label = word_labels[word_idx]
                if label.startswith("B-"):
                    label = label.replace("B-", "I-")
                token_labels.append(label2id[label])

            previous_word_idx = word_idx

        labels.append(token_labels)

    tokenized_inputs["labels"] = labels
    return tokenized_inputs

train_ds = train_ds.map(tokenize_and_align_labels, batched=True)
val_ds   = val_ds.map(tokenize_and_align_labels, batched=True)

# ===== 4. Модель =====
model = AutoModelForTokenClassification.from_pretrained(
    MODEL_NAME,
    num_labels=len(unique_labels),
    id2label=id2label,
    label2id=label2id
)

model.to(device)

# ===== 5. Обучение =====
training_args = TrainingArguments(
    output_dir="./results",
    num_train_epochs=3,
    per_device_train_batch_size=8,
    per_device_eval_batch_size=8,
    learning_rate=2e-5,
    weight_decay=0.01,
    logging_dir="./logs",
    save_strategy="epoch",
    eval_strategy="epoch",  # ← ВАЖНО: исправлено!
    load_best_model_at_end=True,
    metric_for_best_model="eval_loss",
    fp16=torch.cuda.is_available(),
)

trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=train_ds,
    eval_dataset=val_ds,
)

# ===== 6. Train =====
trainer.train()

# ===== 7. Save =====
model.save_pretrained("./model_ner")
tokenizer.save_pretrained("./model_ner")