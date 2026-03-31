from transformers import pipeline

clf = pipeline("text-classification", model="./model", tokenizer="./model")

print(clf("создай задачу купить молоко"))
print(clf("Создай новую задачу выполнить лабораторную работу до 27 декабря 0 часов, приоритет высокий, добавь это в проект ДЗ, и в описании напиши лабораторная по ТАТД"))
print(clf("обнови задачу купить молоко, сегодня дедлайн"))