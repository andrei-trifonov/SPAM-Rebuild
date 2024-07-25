#
# #!/usr/bin/python
# from PIL import Image
# import os, sys
#
# path = 'C:\\Users\\user2\\Downloads\\Resize_tmp\\'
# dirs = os.listdir(path)
#
#
#
# def resize():
#     i = 0
#     for item in dirs:
#         i = i + 1
#         if os.path.isfile(path + item):
#             im = Image.open(path + item)
#             f, e = os.path.splitext(path + item)
#             im = im.convert('RGB')
#             imResize = im.resize((300, 169), Image.ANTIALIAS)
#             imResize.save("C:\\Users\\user2\\Downloads\\Resize_tmp\\new\\" + str(item), 'JPEG', quality=90)
#
#
# resize()



import re


def extract_quotes(file_path):
  """
  Извлекает фразы в кавычках из файла и сохраняет их в отдельный файл.

  Args:
    file_path: Путь к файлу.
  """
  with open(file_path, 'r', encoding='utf-8') as f:
    text = f.read()

  quotes = {}
  quote_count = 1

  quotes = {}
  quote_count = 1

  # Находим фразы в кавычках и заменяем их метками
  while re.search(r'"(.*?)"', text):  # Используем поиск по тексту
      match = re.search(r'"([^"]*[\u0400-\u04FF]+[^"]*)"', text)
      if (match == None):
          break


      quote = match.group(1)
      print(quote)
      quotes[f"QUOTE_{quote_count}"] = quote
      text = text.replace(quote, f"QUOTE_{quote_count}")
      quote_count += 1

  # Сохраняем метки и фразы в отдельный файл
  with open('quotes.txt', 'w', encoding='utf-8') as f:
      for quote_key, quote in quotes.items():
          f.write(f"{quote_key}: {quote}\n")

  # Сохраняем текст с метками в исходный файл
  with open(file_path, 'w', encoding='utf-8') as f:
      f.write(text)

#extract_quotes("dialogue.json")
def replace_quotes(dialogue_file, quotes_file):
  """Заменяет ключи в dialogue_file на значения из quotes_file.

  Args:
    dialogue_file: Путь к файлу с диалогом.
    quotes_file: Путь к файлу со словарем цитат.

  Returns:
    Строку с измененным диалогом.
  """

  # Загружаем словарь цитат
  quotes = {}
  with open(quotes_file, 'r', encoding='utf-8') as f:
    for line in f:
      key, value = line.strip().split(':', 1)
      quotes[key] = value.strip()

  # Загружаем диалог
  with open(dialogue_file, 'r', encoding='utf-8') as f:
    dialogue = f.read()

  # Заменяем ключи на значения
  for key, value in quotes.items():
    dialogue = dialogue.replace(r' {key} ', value ) # Используем replace

  print( dialogue)




# Извлечение фраз и сохранение в файл


# Перевод фраз (замените эту часть на ваш собственный код перевода)
# ...

# Замена меток на переведенные фразы в исходном файле

replace_quotes("dialogue.json", 'quotes.txt')