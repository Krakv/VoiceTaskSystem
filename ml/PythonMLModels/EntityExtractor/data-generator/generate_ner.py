import random
import json
import re
from datetime import datetime, timedelta

# ══════════════════════════════════════════════════════
#  СЛОВАРИ СУЩНОСТЕЙ
# ══════════════════════════════════════════════════════

TITLES = [
    "сделать отчёт", "написать отчёт", "подготовить отчёт", "закончить отчёт",
    "купить молоко", "купить продукты", "сходить в магазин",
    "подготовить презентацию", "сделать презентацию", "сверстать презентацию",
    "позвонить клиенту", "позвонить в поддержку", "созвониться с командой",
    "закончить лабораторную", "сдать лабораторную", "оформить лабораторную",
    "написать письмо", "отправить письмо", "ответить на письмо",
    "заполнить таблицу", "обновить таблицу", "создать таблицу",
    "проверить код", "исправить баг", "написать тесты", "сделать ревью",
    "обновить документацию", "написать README", "оформить документацию",
    "подготовить план", "составить план", "утвердить план",
    "провести митинг", "назначить встречу", "созвать совещание",
    "задеплоить приложение", "выкатить релиз", "обновить сервер",
    "настроить CI/CD", "настроить пайплайн", "поднять окружение",
    "разобраться с логами", "почистить логи", "настроить мониторинг",
    "сделать дизайн", "нарисовать макет", "утвердить макет",
    "написать скрипт", "автоматизировать процесс", "написать автоматизацию",
    "провести интервью", "провести онбординг", "добавить сотрудника",
    "составить отчётность", "сдать квартальный отчёт", "подготовить финансовый отчёт",
]

PROJECTS = [
    "ДЗ", "Проект A", "Проект B", "Проект Альфа", "Проект Бета",
    "Работа", "Учёба", "Личное", "Стартап", "Хакатон",
    "Учебный проект", "Рабочий проект", "Дипломная работа",
    "Клиент X", "Клиент Y", "Основной проект", "Побочный проект",
    "Backend", "Frontend", "Мобильное приложение", "Инфраструктура",
    "HR", "Финансы", "Маркетинг", "Аналитика",
]

PRIORITIES = {
    "низкий":    ["низкий", "низкий приоритет", "некритично", "не срочно", "можно потом"],
    "средний":   ["средний", "средний приоритет", "обычный", "нормальный", "стандартный"],
    "высокий":   ["высокий", "высокий приоритет", "важно", "важный", "приоритетно"],
    "срочный":   ["срочный", "критичный", "критически важно", "срочно", "горит", "asap", "ASAP"],
}
PRIORITY_FLAT = {v: k for k, vals in PRIORITIES.items() for v in vals}

STATUSES = {
    "новая":        ["новая", "не начата", "новый", "свежая"],
    "в работе":     ["в работе", "в процессе", "выполняется", "в прогрессе", "работаю над этим"],
    "на проверке":  ["на проверке", "ждёт ревью", "на ревью", "на согласовании"],
    "выполнена":    ["выполнена", "готово", "сделано", "закрыта", "завершена", "выполнено"],
    "отменена":     ["отменена", "отменить", "отменено", "не нужна"],
}
STATUS_FLAT = {v: k for k, vals in STATUSES.items() for v in vals}

DUE_DATES = [
    "сегодня", "сегодня вечером", "сегодня до 18:00",
    "завтра", "завтра утром", "завтра до обеда",
    "послезавтра",
    "в пятницу", "в понедельник", "в среду", "в четверг",
    "на выходных", "на этой неделе", "на следующей неделе",
    "до конца недели", "до конца месяца", "до конца квартала",
    "через 3 дня", "через неделю", "через месяц",
    "1 января", "15 февраля", "31 марта", "10 апреля",
    "2025-01-15", "2025-02-28", "2025-03-10",
    "до 20:00", "до конца дня", "к утру",
]

DESCRIPTIONS = [
    "лабораторная работа по ТАТД",
    "разобраться с архитектурой",
    "включая тесты и документацию",
    "согласовать с командой",
    "уточнить детали у заказчика",
    "с учётом всех замечаний",
    "сделать по ГОСТ",
    "по шаблону из прошлого года",
    "написать на Python",
    "с покрытием тестами не менее 80%",
    "финальная версия для клиента",
    "черновик для внутреннего использования",
    "срочно нужно для презентации",
    "без ошибок и опечаток",
    "в соответствии с техническим заданием",
]

PARENT_TASKS = [
    "сделать отчёт", "подготовить презентацию", "закончить лабораторную",
    "проверить код", "настроить CI/CD", "задеплоить приложение",
    "провести митинг", "написать документацию", "исправить баги",
]

# ══════════════════════════════════════════════════════
#  ШАБЛОНЫ ФРАЗ-ОБЁРТОК ДЛЯ КАЖДОЙ СУЩНОСТИ
# ══════════════════════════════════════════════════════

TITLE_WRAPPERS = [
    ("создай задачу «{}»",        "TITLE"),
    ("добавь задачу {}",          "TITLE"),
    ("задача — {}",               "TITLE"),
    ("задача: {}",                "TITLE"),
    ("название: {}",              "TITLE"),
    ("{}",                        "TITLE"),
    ("нужно {}",                  "TITLE"),
    ("надо {}",                   "TITLE"),
    ("не забудь {}",              "TITLE"),
    ("задача «{}»",               "TITLE"),
    ("задание — {}",              "TITLE"),
    ("тема задачи: {}",           "TITLE"),
]

PROJECT_WRAPPERS = [
    ("в проекте {}",       "PROJECT"),
    ("проект {}",          "PROJECT"),
    ("для проекта {}",     "PROJECT"),
    ("в рамках {}",        "PROJECT"),
    ("по проекту {}",      "PROJECT"),
    ("добавь в {}",        "PROJECT"),
    ("запиши в {}",        "PROJECT"),
    ("относится к {}",     "PROJECT"),
    ("проект: {}",         "PROJECT"),
    ("в {}",               "PROJECT"),
]

PRIORITY_WRAPPERS = [
    ("приоритет {}",           "PRIORITY"),
    ("с приоритетом {}",       "PRIORITY"),
    ("важность: {}",           "PRIORITY"),
    ("пометь как {}",          "PRIORITY"),
    ("это {}",                 "PRIORITY"),
    ("срочность {}",           "PRIORITY"),
    ("приоритет: {}",          "PRIORITY"),
    ("уровень приоритета {}",  "PRIORITY"),
    ("{}",                     "PRIORITY"),
]

DUE_DATE_WRAPPERS = [
    ("до {}",              "DUE_DATE"),
    ("к {}",               "DUE_DATE"),
    ("дедлайн {}",         "DUE_DATE"),
    ("сдать {}",           "DUE_DATE"),
    ("выполнить {}",       "DUE_DATE"),
    ("срок: {}",           "DUE_DATE"),
    ("дедлайн: {}",        "DUE_DATE"),
    ("нужно {}",           "DUE_DATE"),
    ("сделать {}",         "DUE_DATE"),
    ("завершить {}",       "DUE_DATE"),
    ("готово должно быть {}", "DUE_DATE"),
]

DESCRIPTION_WRAPPERS = [
    ("описание: {}",                   "DESCRIPTION"),
    ("детали: {}",                     "DESCRIPTION"),
    ("суть задачи: {}",                "DESCRIPTION"),
    ("подробнее: {}",                  "DESCRIPTION"),
    ("комментарий: {}",                "DESCRIPTION"),
    ("заметка: {}",                    "DESCRIPTION"),
    ("описание — {}",                  "DESCRIPTION"),
    ("с описанием {}",                 "DESCRIPTION"),
    ("доп. информация: {}",            "DESCRIPTION"),
    ("пояснение: {}",                  "DESCRIPTION"),
]

STATUS_WRAPPERS = [
    ("статус {}",              "STATUS"),
    ("статус: {}",             "STATUS"),
    ("отметь как {}",          "STATUS"),
    ("пометь {}",              "STATUS"),
    ("{}",                     "STATUS"),
    ("задача {}",              "STATUS"),
    ("перевести в {}",         "STATUS"),
    ("поставь статус {}",      "STATUS"),
    ("сменить статус на {}",   "STATUS"),
]

PARENT_WRAPPERS = [
    ("дочерняя к задаче {}",           "PARENT_TASK"),
    ("подзадача «{}»",                 "PARENT_TASK"),
    ("подзадача задачи {}",            "PARENT_TASK"),
    ("как часть задачи {}",            "PARENT_TASK"),
    ("родительская задача — {}",       "PARENT_TASK"),
    ("входит в {}",                    "PARENT_TASK"),
    ("относится к задаче {}",          "PARENT_TASK"),
    ("под задачей {}",                 "PARENT_TASK"),
    ("связана с задачей {}",           "PARENT_TASK"),
    ("является подзадачей {}",         "PARENT_TASK"),
]

# ══════════════════════════════════════════════════════
#  BIO-РАЗМЕТКА
# ══════════════════════════════════════════════════════

def bio_tag(tokens: list[str], entity_tokens: list[str], entity_type: str, labels: list[str]):
    """Находит entity_tokens в tokens и проставляет BIO-метки."""
    n = len(entity_tokens)
    for i in range(len(tokens) - n + 1):
        if tokens[i:i+n] == entity_tokens:
            if labels[i] == "O":  # не перезаписываем уже помеченное
                labels[i] = f"B-{entity_type}"
                for j in range(1, n):
                    labels[i+j] = f"I-{entity_type}"
                return labels
    return labels

def tokenize(text: str) -> list[str]:
    """Простая токенизация с сохранением пунктуации."""
    return re.findall(r'[«»:,!?.]+|[\w\-\/]+', text)

# ══════════════════════════════════════════════════════
#  СБОРКА ПРИМЕРА
# ══════════════════════════════════════════════════════

def pick(wrappers, value):
    """Выбирает случайный wrapper и подставляет value."""
    tmpl, ent_type = random.choice(wrappers)
    phrase = tmpl.format(value)
    return phrase, ent_type, value

def make_sentence(force_entities: list[str] | None = None) -> dict:
    """
    Генерирует одно предложение со случайным набором сущностей.
    force_entities — список типов сущностей, которые ОБЯЗАТЕЛЬНО должны войти.
    """
    chosen_types = set(force_entities or [])

    # Добавляем случайные сущности
    candidates = ["TITLE", "PROJECT", "PRIORITY", "DUE_DATE", "DESCRIPTION", "STATUS", "PARENT_TASK"]
    # TITLE всегда присутствует с вероятностью 85%
    if random.random() < 0.85:
        chosen_types.add("TITLE")
    for t in candidates[1:]:
        if random.random() < 0.45:
            chosen_types.add(t)

    # Минимум 1 сущность
    if not chosen_types:
        chosen_types.add("TITLE")

    # Строим части фразы
    parts = []   # (phrase_str, entity_type, raw_value)

    if "TITLE" in chosen_types:
        parts.append(pick(TITLE_WRAPPERS, random.choice(TITLES)))
    if "PROJECT" in chosen_types:
        parts.append(pick(PROJECT_WRAPPERS, random.choice(PROJECTS)))
    if "PRIORITY" in chosen_types:
        pvals = [v for vals in PRIORITIES.values() for v in vals]
        parts.append(pick(PRIORITY_WRAPPERS, random.choice(pvals)))
    if "DUE_DATE" in chosen_types:
        parts.append(pick(DUE_DATE_WRAPPERS, random.choice(DUE_DATES)))
    if "DESCRIPTION" in chosen_types:
        parts.append(pick(DESCRIPTION_WRAPPERS, random.choice(DESCRIPTIONS)))
    if "STATUS" in chosen_types:
        svals = [v for vals in STATUSES.values() for v in vals]
        parts.append(pick(STATUS_WRAPPERS, random.choice(svals)))
    if "PARENT_TASK" in chosen_types:
        parts.append(pick(PARENT_WRAPPERS, random.choice(PARENT_TASKS)))

    # Перемешиваем части (кроме TITLE — он чаще стоит первым)
    title_parts  = [p for p in parts if p[1] == "TITLE"]
    other_parts  = [p for p in parts if p[1] != "TITLE"]
    random.shuffle(other_parts)

    # 70% — TITLE в начале, 30% — в случайном месте
    if title_parts and random.random() < 0.70:
        ordered = title_parts + other_parts
    else:
        ordered = other_parts + title_parts
        random.shuffle(ordered)

    # Соединяем разделителями
    separators = [", ", "; ", " — ", " | ", ". ", " "]
    sep = random.choice(separators)
    text = sep.join(p[0] for p in ordered)

    # Лёгкий шум
    text = add_noise(text)

    # Токенизация и BIO
    tokens = tokenize(text)
    labels = ["O"] * len(tokens)

    for phrase, ent_type, raw_value in ordered:
        # Ищем raw_value в тексте (именно значение, без обёртки)
        val_tokens = tokenize(raw_value)
        labels = bio_tag(tokens, val_tokens, ent_type, labels)

    return {
        "text":   text,
        "tokens": tokens,
        "labels": labels,
    }

# ══════════════════════════════════════════════════════
#  ШУМ
# ══════════════════════════════════════════════════════

TYPOS = [
    ("задачу", "задачу"), ("проект", "прект"), ("приоритет", "приоретет"),
    ("описание", "описание"), ("срочно", "срчоно"), ("создай", "создай"),
    ("выполнить", "выполнить"), ("готово", "готова"), ("срок", "срок"),
]

def add_noise(text: str) -> str:
    # Случайный регистр (10%)
    if random.random() < 0.10:
        text = text.lower()
    # Двойные пробелы (5%)
    if random.random() < 0.05:
        text = text.replace(" ", "  ", 1)
    # Восклицательный знак (15%)
    if random.random() < 0.15:
        text += random.choice(["!", "!!", "..."])
    # Опечатка (5%)
    if random.random() < 0.05:
        for correct, wrong in TYPOS:
            if correct in text:
                text = text.replace(correct, wrong, 1)
                break
    return text

# ══════════════════════════════════════════════════════
#  ГЕНЕРАЦИЯ
# ══════════════════════════════════════════════════════

def generate(n: int = 50000) -> list[dict]:
    data = []
    entity_types = ["TITLE", "PROJECT", "PRIORITY", "DUE_DATE", "DESCRIPTION", "STATUS", "PARENT_TASK"]

    # Гарантируем минимальное покрытие каждого типа (~3000 на тип)
    per_type = n // (len(entity_types) * 2)
    for ent in entity_types:
        for _ in range(per_type):
            data.append(make_sentence(force_entities=[ent]))

    # Остальное — полностью случайно
    remaining = n - len(data)
    for _ in range(remaining):
        data.append(make_sentence())

    random.shuffle(data)
    return data

# ══════════════════════════════════════════════════════
#  MAIN
# ══════════════════════════════════════════════════════

if __name__ == "__main__":
    print("Генерация датасета...")
    dataset = generate(n=50000)

    output_path = "ner_task_dataset.json"
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(dataset, f, ensure_ascii=False, indent=2)

    # Статистика
    from collections import Counter
    label_counts = Counter()
    empty_labels = 0
    for item in dataset:
        labels = item["labels"]
        label_counts.update(l for l in labels if l != "O")
        if all(l == "O" for l in labels):
            empty_labels += 1

    print(f"\n✅ Сгенерировано примеров: {len(dataset)}")
    print(f"⚠️  Примеров без сущностей:  {empty_labels}")
    print(f"\nРаспределение B-тегов:")
    for tag, cnt in sorted(label_counts.items()):
        if tag.startswith("B-"):
            print(f"  {tag:<25} {cnt:>6}")

    print(f"\nПримеры из датасета:")
    for item in random.sample(dataset, 5):
        print(f"\n  text:   {item['text']}")
        print(f"  tokens: {item['tokens']}")
        print(f"  labels: {item['labels']}")
