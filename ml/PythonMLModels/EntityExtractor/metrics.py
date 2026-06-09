from transformers import AutoTokenizer, AutoModelForTokenClassification
import torch

MODEL_PATH = "./model_ner"
tokenizer = AutoTokenizer.from_pretrained(MODEL_PATH)
model = AutoModelForTokenClassification.from_pretrained(MODEL_PATH)
model.eval()

id2label = model.config.id2label

def predict_ner(text):
    inputs = tokenizer(text, return_tensors="pt", truncation=True, max_length=64)
    with torch.no_grad():
        outputs = model(**inputs)
    
    predictions = torch.argmax(outputs.logits, dim=2)[0]
    tokens = tokenizer.convert_ids_to_tokens(inputs["input_ids"][0])
    
    entities = {}
    current_entity = None
    current_tokens = []
    
    for token, pred_id in zip(tokens, predictions):
        if token in ["<s>", "</s>", "<pad>"]:
            continue
        label = id2label[pred_id.item()]
        if label.startswith("B-"):
            if current_entity:
                entities[current_entity] = tokenizer.convert_tokens_to_string(current_tokens)
            current_entity = label[2:]
            current_tokens = [token]
        elif label.startswith("I-") and current_entity:
            current_tokens.append(token)
        else:
            if current_entity:
                entities[current_entity] = tokenizer.convert_tokens_to_string(current_tokens)
                current_entity = None
                current_tokens = []
    
    return entities

# Тестовые примеры
tests = [
    "создай задачу сделать отчёт до пятницы, приоритет высокий ывв ",
    "добавь задачу купить молоко в проект Личное ыва ",
    "задача написать тесты, дедлайн завтра, описание: с покрытием 80%  авыа",
    "создай срочную задачу позвонить клиенту для проекта Работа ыва",
    "нужно подготовить презентацию до конца недели, приоритет средний в",
    "создай задачу подготовить презентацию, проект Учёба, приоритет высокий, дедлайн завтра к 14:00, описание добавить графики по продажам, статус новая ыва",
    "Создай задачу посчитать деньги через 1 час, приоритет поставь высокий, проект Продукты, поставь родительскую задачу купить молоко ва",
]

for text in tests:
    print(f"\nТекст: {text}")
    print(f"Сущности: {predict_ner(text)}")