# rx-template-settings
Репозиторий с типовой разработкой «Прикладные константы».

## Описание
Решение позволяет устанавливать и хранить параметры, необходимые для работы системы, но которые не могут быть явно заданы в коде из-за возможной необходимости их изменения администратором.

Состав объектов разработки:
* Справочник "Прикладные константы"

## Варианты расширения функциональности на проектах
1.	Добавление валидации вводимых значений для конкретных записей в случае необходимости. Проверка на mail, количество знаков, это дата, это число. Для предупреждения ввода неверных данных.
2.	Добавление скрытия некоторых значений, либо их хранение в скрытом виде (в основном для паролей).
3.	Добавление полей для хранения значений в конкретном типе данных, не строкой. Либо добавление табличной части.

## Порядок установки
Для работы требуется установленный Directum RX версии 3.6 или выше. 

### Установка для ознакомления
1. Склонировать репозиторий rx-template-settings в папку.
2. Указать в _ConfigSettings.xml DDS:
```xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" />
  <repository folderName="RX" solutionType="Base" url="<адрес локального репозитория>" />
  <repository folderName="<Папка из п.1>" solutionType="Work" 
     url="https://github.com/DirectumCompany/rx-template-settings" />
</block>
```

### Установка для использования на проекте
Возможные варианты:

**A. Fork репозитория**
1. Сделать fork репозитория rx-template-settings для своей учетной записи.
2. Склонировать созданный в п. 1 репозиторий в папку.
3. Указать в _ConfigSettings.xml DDS:
``` xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.2>" solutionType="Work" 
     url="<Адрес репозитория gitHub учетной записи пользователя из п. 1>" />
</block>
```

**B. Подключение на базовый слой.**

Вариант не рекомендуется, так как при выходе версии шаблона разработки не гарантируется обратная совместимость.
1. Склонировать репозиторий rx-template-settings в папку.
2. Указать в _ConfigSettings.xml DDS:
``` xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.1>" solutionType="Base" 
     url="<Адрес репозитория gitHub>" />
  <repository folderName="<Папка для рабочего слоя>" solutionType="Work" 
     url="https://github.com/DirectumCompany/rx-template-settings" />
</block>
```

**C. Копирование репозитория в систему контроля версий.**

Рекомендуемый вариант для проектов внедрения.
1. В системе контроля версий с поддержкой git создать новый репозиторий.
2. Склонировать репозиторий rx-template-settings в папку с ключом `--mirror`.
3. Перейти в папку из п. 2.
4. Импортировать клонированный репозиторий в систему контроля версий командой:

`git push –mirror <Адрес репозитория из п. 1>`

