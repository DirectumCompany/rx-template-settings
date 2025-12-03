using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DirRX.AppliedConstants.Server
{
  public partial class ModuleFunctions
  {
    #region Общие функции.
    
    /// <summary>
    /// Получить константу по Guid.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Константа.</returns>
    [Public, Remote(IsPure = true)]
    public virtual IConstantsSetting GetСonstant(string guid)
    {
      var constant = ConstantsSettings.GetAll(p => p.Guid == guid).FirstOrDefault();
      if (constant == null)
        Logger.ErrorFormat("AppliedConstants. GetСonstant. Guid: \"{0}\". Константы с таким guid не существует в системе.", guid);
      
      return constant;
    }
    
    /// <summary>
    /// Проверить тип константы.
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <param name="type">Тип константы.</param>
    /// <returns>Результат проверки типа. True - если ожидаемый тип соответствует типу прикладной константы, иначе - false.</returns>
    [Public, Remote(IsPure = true)]
    public virtual bool CheckConstantType(IConstantsSetting constant, Nullable<Enumeration> type)
    {
      if (constant == null)
        return false;
      
      var isCorrectType = constant.Type == type;
      if (!isCorrectType)
        Logger.ErrorFormat("AppliedConstants. CheckConstantType. Имя: \"{0}\". Guid: \"{1}\". Тип константы не соответствует. Ожидаемый: {2}. Фактический: {3}.", constant.Name, constant.Guid,
                           ConstantsSettings.Info.Properties.Type.GetLocalizedValue(type), ConstantsSettings.Info.Properties.Type.GetLocalizedValue(constant.Type));
      
      return isCorrectType;
    }
    
    /// <summary>
    /// Преобразовать строковое значение в список строк.
    /// </summary>
    /// <param name="value">Строка с элементами, разделёнными разделителем.</param>
    /// <returns>Список строк без пустых элементов и с обрезанными пробелами.</returns>
    [Public, Remote(IsPure = true)]
    public static List<string> ParseStringList(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        Logger.Error("AppliedConstants. ParseStringList. Пустое или некорректное строковое значение.");
        return new List<string>();
      }
      
      // Символ-разделитель, по умолчанию запятая.
      var separator = Constants.Module.ConstantListSeparator;
      
      return value
        .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrEmpty(x))
        .ToList();
    }
    
    /// <summary>
    /// Преобразовать строковое значение в список целых чисел.
    /// </summary>
    /// <param name="value">Строка со значениями, разделёнными запятыми.</param>
    /// <returns>Список целых чисел.</returns>
    [Public, Remote(IsPure = true)]
    public static List<int> ParseIntegerList(string value)
    {
      var result = new List<int>();
      var items = ParseStringList(value);

      foreach (var item in items)
      {
        int parsed;
        if (int.TryParse(item, out parsed))
          result.Add(parsed);
        else
          Logger.ErrorFormat("AppliedConstants. ParseIntList. Не удалось преобразовать \"{0}\" в int.", item);
      }

      return result;
    }

    #endregion
    
    #region Функции для создания системных прикладных констант и групп констант.

    /// <summary>
    /// Создать системную прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <returns>Новая или обновленная константа.</returns>
    /// <remarks>Выполняется поиск константы по Guid. Если константа найдена, то выполняется её обновление, иначе создаётся новая.
    /// Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    public virtual IConstantsSetting CreateConstant(string name, string description, string guid, string groupGuid)
    {
      var logPrefix = string.Format("AppliedConstants. CreateConstant. Name: \"{0}\". Guid: \"{1}\". GroupGuid: \"{2}\"", name, guid, groupGuid);
      Logger.DebugFormat("{0}. Создание/обновление системной прикладной константы.", logPrefix);
      
      // Валидация вх. переменных.
      if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(groupGuid))
      {
        Logger.ErrorFormat("{0}. Не заполнены обязательные поля для константы.", logPrefix);
        return ConstantsSettings.Null;
      }
      
      // Найти константу по guid или создать новую.
      var constant = ConstantsSettings.GetAll(p => p.Guid == guid).FirstOrDefault() ?? ConstantsSettings.Create();
      
      // Указать guid константы.
      if (string.IsNullOrEmpty(constant.Guid))
        constant.Guid = guid;
      
      // Указать признак "Системная".
      var isSystem = true;
      if (constant.IsSystem != isSystem)
        constant.IsSystem = isSystem;
      
      // Указать группу констант.
      if (constant.ConstantsGroup == null)
      {
        constant.ConstantsGroup = ConstantsGroups.GetAll(c => c.Guid == groupGuid).FirstOrDefault();
        if (constant.ConstantsGroup == null)
        {
          Logger.ErrorFormat("{0}. Не найдена группа констант по guid.", logPrefix);
          return ConstantsSettings.Null;
        }
      }
      
      // Если изменилось системное имя константы, то обновляем его у константы.
      if (constant.Name != name)
        constant.Name = name;
      
      // Если изменилось примечание, то обновляем его у константы.
      if (constant.Description != description)
        constant.Description = description;
      
      return constant;
    }
    
    /// <summary>
    /// Создать системную группу констант.
    /// </summary>
    /// <param name="name">Имя группы констант.</param>
    /// <param name="description">Описание.</param>
    /// <param name="Guid">Guid группы констант.</param>
    /// <remarks>Для системных групп констант пользователю недоступно для изменения поле Guid.</remarks>
    [Public, Remote]
    public void CreateConstantsGroup(string name, string description, string guid)
    {
      Logger.DebugFormat("AppliedConstants. CreateConstantsGroup. Name: \"{0}\". Guid: \"{1}\". Создание/обновление системной группы констант.", name, guid);
      
      var сonstantsGroup = ConstantsGroups.GetAll(p => p.Guid == guid).FirstOrDefault() ?? ConstantsGroups.Create();
      if (сonstantsGroup.Name != name)
        сonstantsGroup.Name = name;
      if (сonstantsGroup.Description != description)
        сonstantsGroup.Description = description;
      if (сonstantsGroup.Guid != guid)
        сonstantsGroup.Guid = guid;
      
      // Указать признак "Системная".
      var isSystem = true;
      if (сonstantsGroup.IsSystem != isSystem)
        сonstantsGroup.IsSystem = isSystem;
      if (сonstantsGroup.State.IsChanged)
        сonstantsGroup.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - строка).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Строковое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, string value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может указать своё значение.
      if (constant.State.IsInserted)
      {
        constant.StringValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.StringType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - вещественное число).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Вещественное число.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, double value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может указать своё значение.
      if (constant.State.IsInserted)
      {
        constant.DoubleValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.DoubleType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - идентификатор).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Значение идентификатора.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создается новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, long value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может указать своё значение.
      if (constant.State.IsInserted)
      {
        constant.IdentifierValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.IdentifierType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - целое число).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Целое число.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, int value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании, т.к. пользователь может задать своё значение.
      if (constant.State.IsInserted)
      {
        constant.IntegerValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.IntegerType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - дата).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Дата.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, DateTime value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может указать своё значение.
      if (constant.State.IsInserted)
      {
        constant.DateValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.DateType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - пароль).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Строковое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreatePasswordConstant(string name, string description, string value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может установить своё значение для константы.
      if (constant.State.IsInserted)
      {
        constant.PasswordValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.PasswordType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - логический).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="value">Логическое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string description, bool value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, description, guid, groupGuid);
      if (constant == null)
        return;
      
      // Тип и значение константы задаётся только при создании константы, т.к. пользователь может указать своё значение.
      if (constant.State.IsInserted)
      {
        constant.BoolValue = value;
        constant.Type = AppliedConstants.ConstantsSetting.Type.BoolType;
      }
      
      if (constant.State.IsChanged)
        constant.Save();
    }
    
    #endregion
    
    #region Функции для извлечения значений из прикладных констант.
    
    /// <summary>
    /// Получить значение прикладной константы (строка).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual string GetStringValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetStringValue(constant);
    }

    /// <summary>
    /// Получить значение прикладной константы (вещественное число).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual double? GetDoubleValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetDoubleValue(constant);
    }

    /// <summary>
    /// Получить значение прикладной константы (целое число).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual int? GetIntegerValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetIntegerValue(constant);
    }

    /// <summary>
    /// Получить значение прикладной константы (идентификатор).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual long? GetIdentifierValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetIdentifierValue(constant);
    }

    /// <summary>
    /// Получить значение прикладной константы (дата).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual DateTime? GetDateValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetDateValue(constant);
    }


    /// <summary>
    /// Получить значение прикладной константы (логическое).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual bool? GetBoolValue(string guid)
    {
      var constant = GetСonstant(guid);
      return GetBoolValue(constant);
    }
    
    /// <summary>
    /// Получить значение прикладной константы (пароль).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [Public, Remote(IsPure = true)]
    public virtual string GetPasswordValue(string guid)
    {
      var constant = GetСonstant(guid);
      if (!CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.PasswordType))
        return string.Empty;

      if (string.IsNullOrEmpty(constant.Value))
      {
        Logger.DebugFormat("AppliedConstants. GetPasswordValue. Name: \"{0}\". Guid: \"{1}\". Значение прикладной константы не заполнено.", constant.Name, constant.Guid);
        return string.Empty;
      }
      
      return Encryption.Decrypt(constant.Value);
    }
    
    /// <summary>
    /// Получить список строк из прикладной константы.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <remarks>Значения хранятся как строка через разделитель. Символ-разделитель по умолчанию - запятая.</remarks>
    [Public, Remote(IsPure = true)]
    public virtual List<string> GetStringListValue(string guid)
    {
      var constantValue = this.GetStringValue(guid);
      return ParseStringList(constantValue);
    }

    /// <summary>
    /// Получить список целых чисел из прикладной константы.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <remarks>Значения хранятся как строка через разделитель. Символ-разделитель по умолчанию - запятая.</remarks>
    [Public, Remote(IsPure = true)]
    public virtual List<int> GetIntegerListValue(string guid)
    {
      var constantValue = this.GetStringValue(guid);
      return ParseIntegerList(constantValue);
    }
    
    #endregion
    
    #region Функции вычисляемых выражений.
    
    /// <summary>
    /// Получить константу по Guid.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Константа.</returns>
    [ExpressionElement("ExpressionElement_GetConstant_Name", "ExpressionElement_GetConstant_Description", "ExpressionElement_GetConstant_GuidParam")]
    public static IConstantsSetting GetСonstant(Sungero.Workflow.ITask task, string guid)
    {
      return AppliedConstants.PublicFunctions.Module.Remote.GetСonstant(guid);
    }
    
    /// <summary>
    /// Получить константу по Guid.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Константа.</returns>
    [ExpressionElement("ExpressionElement_GetConstant_Name", "ExpressionElement_GetConstant_Description", "ExpressionElement_GetConstant_GuidParam")]
    public static IConstantsSetting GetСonstant(Sungero.Workflow.IAssignment assignment, string guid)
    {
      return AppliedConstants.PublicFunctions.Module.Remote.GetСonstant(guid);
    }
    
    /// <summary>
    /// Получить константу по Guid.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Константа.</returns>
    [ExpressionElement("ExpressionElement_GetConstant_Name", "ExpressionElement_GetConstant_Description", "ExpressionElement_GetConstant_GuidParam")]
    public static IConstantsSetting GetСonstant(Sungero.Content.IElectronicDocument document, string guid)
    {
      return AppliedConstants.PublicFunctions.Module.Remote.GetСonstant(guid);
    }
    
    /// <summary>
    /// Получить константу по Guid.
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Константа.</returns>
    [ExpressionElement("ExpressionElement_GetConstant_Name", "ExpressionElement_GetConstant_Description", "ExpressionElement_GetConstant_GuidParam")]
    public static IConstantsSetting GetСonstant(Sungero.CoreEntities.IDatabookEntry databook, string guid)
    {
      return AppliedConstants.PublicFunctions.Module.Remote.GetСonstant(guid);
    }
    
    /// <summary>
    /// Получить значение прикладной константы (строка).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetStringValue_Name", "ExpressionElement_GetStringValue_Description")]
    public static string GetStringValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.StringType))
        return null;
      
      if (string.IsNullOrEmpty(constant.StringValue))
        Logger.DebugFormat("AppliedConstants. GetStringValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
      
      return constant.StringValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (вещественное число).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetDoubleValue_Name", "ExpressionElement_GetDoubleValue_Description")]
    public static double? GetDoubleValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.DoubleType))
        return null;
      
      if (constant.DoubleValue == null)
        Logger.DebugFormat("AppliedConstants. GetDoubleValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
      
      return constant.DoubleValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (целое число).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetIntegerValue_Name", "ExpressionElement_GetIntegerValue_Description")]
    public static int? GetIntegerValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;

      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.IntegerType))
        return null;
      
      if (constant.IntegerValue == null)
        Logger.DebugFormat("AppliedConstants. GetIntegerValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
      
      return constant.IntegerValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (идентификатор).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetIdentifierValue_Name", "ExpressionElement_GetIdentifierValue_Description")]
    public static long? GetIdentifierValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.IdentifierType))
        return null;
      
      if (constant.IdentifierValue == null)
        Logger.DebugFormat("AppliedConstants. GetIdentifierValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
      
      return constant.IdentifierValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (дата).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetDateValue_Name", "ExpressionElement_GetDateValue_Description")]
    public static DateTime? GetDateValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.DateType))
        return null;
      
      if (constant.DateValue == null)
      {
        Logger.DebugFormat("AppliedConstants. GetDateValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
        return null;
      }
      
      return constant.DateValue.Value;
    }


    /// <summary>
    /// Получить значение прикладной константы (логическое).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetBoolValue_Name", "ExpressionElement_GetBoolValue_Description")]
    public static bool? GetBoolValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.BoolType))
        return null;
      
      if (constant.BoolValue == null)
        Logger.DebugFormat("AppliedConstants. GetBoolValue. Name: \"{0}\". Guid: \"{1}\". Не заполнено значение прикладной константы.", constant.Name, constant.Guid);
      
      return constant.BoolValue;
    }
    
    /// <summary>
    /// Получить значение прикладной константы (список строк).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetStringListValue_Name", "ExpressionElement_GetStringListValue_Description")]
    public static List<string> GetStringListValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.StringType))
        return null;
      
      // Преобразовать строковое значение константы в список строк.
      return ParseStringList(constant.StringValue);
    }
    
    /// <summary>
    /// Получить значение прикладной константы (список целых чисел).
    /// </summary>
    /// <param name="constant">Константа.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetIntListValue_Name", "ExpressionElement_GetIntListValue_Description")]
    public static List<int> GetIntegerListValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.StringType))
        return null;
      
      // Преобразовать строковое значение константы в список целых чисел.
      return ParseIntegerList(constant.StringValue);
    }
    
    #endregion
  }
}