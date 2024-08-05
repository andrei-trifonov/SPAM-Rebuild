import json
import re

# Загружаем JSON данные
with open('dialogueDay1.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# Извлекаем все фразы
phrases = []
quotes = {}
quote_number = 1
for label in data['Labels']:
    for line_data in label['lines']:
          if (line_data['line'] != ""):
            phrases.append(line_data['line'])
            quotes[f"QUOTE_{quote_number}"] = line_data['line']
            line_data['line'] = "QUOTE_" + str(quote_number) 
            quote_number+=1
# Выводим полученные фразы
with open('QuottedText.json', 'w') as f:
    json.dump(data, f)
with open("Quotes.json", "w", encoding="utf-8") as f:
    json.dump(quotes, f, ensure_ascii=False)









import json
import re

# Загружаем JSON данные
with open('QuottedText.json', 'r', encoding='utf-8') as f:
    data = json.load(f)
with open("Quotes.json", "r", encoding="utf-8") as f:
    my_dict = json.load(f)

# Print the loaded dictionary
 
phrases = []

quote_number = 1
for label in data['Labels']:
    for line_data in label['lines']:
          if (line_data['line'] != ""):
            line_data['line']=quotes[line_data['line']]
          
# Выводим полученные фразы
with open('TranslatedText.json', 'w', encoding="utf-8") as f:
    json.dump(data, f, ensure_ascii=False)
