import pandas as pd
import torch
from datasets import Dataset
from transformers import (
    AutoTokenizer,
    AutoModelForSequenceClassification,
    TrainingArguments,
    Trainer,
    EarlyStoppingCallback
)
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score, f1_score, classification_report

labels = [
    "TASK_CREATE",
    "TASK_UPDATE",
    "TASK_DELETE",
    "TASK_QUERY",
    "UNKNOWN",
    "AMBIGUOUS"
]

MODEL_NAME = "cointegrated/rubert-tiny2"

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

df = pd.read_csv("data.csv")

label2id = {
    "TASK_CREATE": 0,
    "TASK_UPDATE": 1,
    "TASK_DELETE": 2,
    "TASK_QUERY": 3,
    "UNKNOWN": 4,
    "AMBIGUOUS": 5
}
id2label = {v: k for k, v in label2id.items()}

df["label"] = df["label"].map(label2id)

train_df, temp_df = train_test_split(df, test_size=0.2, random_state=42)
val_df, test_df = train_test_split(temp_df, test_size=0.5, random_state=42)

train_ds = Dataset.from_pandas(train_df.reset_index(drop=True))
val_ds   = Dataset.from_pandas(val_df.reset_index(drop=True))
test_ds  = Dataset.from_pandas(test_df.reset_index(drop=True))

tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)

def tokenize(example):
    return tokenizer(
        example["text"],
        truncation=True,
        padding="max_length",
        max_length=64
    )

train_ds = train_ds.map(tokenize, batched=True)
val_ds   = val_ds.map(tokenize, batched=True)
test_ds  = test_ds.map(tokenize, batched=True)

train_ds.set_format(type="torch", columns=["input_ids", "attention_mask", "label"])
val_ds.set_format(type="torch", columns=["input_ids", "attention_mask", "label"])
test_ds.set_format(type="torch", columns=["input_ids", "attention_mask", "label"])

model = AutoModelForSequenceClassification.from_pretrained(
    MODEL_NAME,
    num_labels=6,
    id2label=id2label,
    label2id=label2id
)

model.to(device)

def compute_metrics(eval_pred):
    logits, labels = eval_pred
    preds = logits.argmax(axis=1)
    return {
        "accuracy": accuracy_score(labels, preds),
        "f1": f1_score(labels, preds, average="weighted")
    }

training_args = TrainingArguments(
    output_dir="./results",
    learning_rate=2e-5,
    per_device_train_batch_size=16,
    per_device_eval_batch_size=16,
    num_train_epochs=10,
    eval_strategy="epoch",
    save_strategy="epoch",
    logging_dir="./logs",
    warmup_ratio=0.1,
    weight_decay=0.01,
    load_best_model_at_end=True,
    metric_for_best_model="eval_loss",
    greater_is_better=False,
    fp16=torch.cuda.is_available(),
    logging_steps=50
)

trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=train_ds,
    eval_dataset=val_ds,
    compute_metrics=compute_metrics,
    callbacks=[EarlyStoppingCallback(early_stopping_patience=2)]
)

trainer.train()

model.save_pretrained("./model")
tokenizer.save_pretrained("./model")

metrics = trainer.evaluate(test_ds)
print(metrics)

preds = trainer.predict(test_ds)
y_pred = preds.predictions.argmax(axis=1)

print(classification_report(test_df["label"], y_pred))

model.eval()

dummy_input = tokenizer(
    "тестовое предложение",
    return_tensors="pt",
    padding="max_length",
    truncation=True,
    max_length=64
)

torch.onnx.export(
    model,
    (dummy_input["input_ids"].to(device), dummy_input["attention_mask"].to(device)),
    "model.onnx",
    input_names=["input_ids", "attention_mask"],
    output_names=["logits"],
    dynamic_axes={
        "input_ids": {0: "batch_size"},
        "attention_mask": {0: "batch_size"},
        "logits": {0: "batch_size"}
    },
    opset_version=14
)