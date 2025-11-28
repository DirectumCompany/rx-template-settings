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
        Logger.ErrorFormat("GetСonstant. Guid: {0}. Константы с таким guid не существует в системе.", guid);
      
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
        Logger.ErrorFormat("CheckConstantType. Имя: {0}. Guid: {1}. Тип константы не соответствует. Ожидаемый: {2}. Фактический: {3}.", constant.Name, constant.Guid,
                           ConstantsSettings.Info.Properties.Type.GetLocalizedValue(type), ConstantsSettings.Info.Properties.Type.GetLocalizedValue(constant.Type));
      
      return isCorrectType;
    }

    #endregion
    
    #region Функции для создания системных прикладных констант и групп констант.

    /// <summary>
    /// Создать системную прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <returns>Новая или обновленная константа.</returns>
    /// <remarks>Выполняется поиск константы по Guid. Если константа найдена, то выполняется её обновление, иначе создаётся новая.
    /// Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    public virtual IConstantsSetting CreateConstant(string name, string guid, string groupGuid)
    {
      var logPrefix = string.Format("CreateConstant. Name: {0}. Guid: {1}. GroupGuid: {2}", name, guid, groupGuid);
      Logger.DebugFormat("{0}. Создание прикладной константы.", logPrefix);
      
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
      
      // Если константа не сохранена в БД, заполняем её поля.
      if (constant.State.IsInserted)
        constant.Name = name;
      
      return constant;
    }
    
    /// <summary>
    /// Создать системную группу констант.
    /// </summary>
    /// <param name="name">Имя группы констант.</param>
    /// <param name="Guid">Guid группы констант.</param>
    /// <remarks>Для системных групп констант пользователю недоступно для изменения поле Guid.</remarks>
    [Public, Remote]
    public void CreateConstantsGroup(string name, string guid)
    {
      if (ConstantsGroups.GetAll(p => p.Guid == guid).Any())
        return;
      
      var newConstantsGroup = ConstantsGroups.Create();
      newConstantsGroup.Name = name;
      newConstantsGroup.Guid = guid;
      newConstantsGroup.IsSystem = true;
      newConstantsGroup.Save();
    }

    /// <summary>
    /// Создать системную прикладную константу (Тип - строка).
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="value">Строковое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, string value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Вещественное число.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, double value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Значение идентификатора.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создается новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, long value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Целое число.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, int value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Дата.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, DateTime value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Строковое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreatePasswordConstant(string name, string value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
    /// <param name="value">Логическое значение.</param>
    /// <param name="guid">Guid константы.</param>
    /// <param name="groupGuid">Guid группы константы.</param>
    /// <remarks>Выполняется поиск константы по Guid. Если константа не найдена, то создаётся новая. Для системных констант пользователю недоступны для изменения поля (Имя, Guid, Группа констант, Тип).</remarks>
    [Public, Remote]
    public virtual void CreateConstant(string name, bool value, string guid, string groupGuid)
    {
      var constant = CreateConstant(name, guid, groupGuid);
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
      if (!CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.StringType))
        return null;
      
      return constant.StringValue;
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
        return null;

      return Encryption.Decrypt(constant.Value);
    }
    
    #endregion
    
    #region Функции вычисляемых выражений.
    
    /// <summary>
    /// Получить значение прикладной константы (строка).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetStringValue_Name", "ExpressionElement_GetStringValue_Description")]
    public static string GetStringValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.StringType))
        return null;
      
      return constant.StringValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (вещественное число).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetDoubleValue_Name", "ExpressionElement_GetDoubleValue_Description")]
    public static double? GetDoubleValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.DoubleType))
        return null;
      
      return constant.DoubleValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (целое число).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetIntegerValue_Name", "ExpressionElement_GetIntegerValue_Description")]
    public static int? GetIntegerValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;

      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.IntegerType))
        return null;
      
      return constant.IntegerValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (идентификатор).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetIdentifierValue_Name", "ExpressionElement_GetIdentifierValue_Description")]
    public static long? GetIdentifierValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.IdentifierType))
        return null;
      
      return constant.IdentifierValue;
    }

    /// <summary>
    /// Получить значение прикладной константы (дата).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetDateValue_Name", "ExpressionElement_GetDateValue_Description")]
    public static DateTime? GetDateValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.DateType))
        return null;
      
      return constant.DateValue.Value;
    }


    /// <summary>
    /// Получить значение прикладной константы (логическое).
    /// </summary>
    /// <param name="guid">Guid константы.</param>
    /// <returns>Значение константы.</returns>
    [ExpressionElement("ExpressionElement_GetBoolValue_Name", "ExpressionElement_GetBoolValue_Description")]
    public static bool? GetBoolValue(IConstantsSetting constant)
    {
      if (constant == null)
        return null;
      
      if (!AppliedConstants.PublicFunctions.Module.Remote.CheckConstantType(constant, AppliedConstants.ConstantsSetting.Type.BoolType))
        return null;
      
      return constant.BoolValue;
    }
    
    #endregion
  }
}