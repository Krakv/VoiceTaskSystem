import torch
from transformers import AutoTokenizer, AutoModelForTokenClassification

MODEL_PATH = "./model_ner"
ONNX_PATH = "./model.onnx"

# ===== загрузка =====
tokenizer = AutoTokenizer.from_pretrained(MODEL_PATH)
model = AutoModelForTokenClassification.from_pretrained(MODEL_PATH)

model.eval()

# ===== dummy input =====
dummy_text = ["Пример предложения для экспорта"]

inputs = tokenizer(
    dummy_text,
    return_tensors="pt",
    padding="max_length",
    truncation=True,
    max_length=64
)

# ===== экспорт =====
torch.onnx.export(
    model,
    (inputs["input_ids"], inputs["attention_mask"]),
    ONNX_PATH,
    input_names=["input_ids", "attention_mask"],
    output_names=["logits"],
    dynamic_axes={
        "input_ids": {0: "batch_size", 1: "sequence"},
        "attention_mask": {0: "batch_size", 1: "sequence"},
        "logits": {0: "batch_size", 1: "sequence"}
    },
    opset_version=14
)

print(f"ONNX модель сохранена в {ONNX_PATH}")