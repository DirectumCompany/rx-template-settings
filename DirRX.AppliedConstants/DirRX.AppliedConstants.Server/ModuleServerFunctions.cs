using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DirRX.AppliedConstants.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="isSystem">Системная.</param>
    /// <param name="value">Строковое значение.</param>
    [Public, Remote]
    public void CreateConstant(string name, bool isSystem, string value)
    {
      CreateConstant(name, isSystem, value, null, null, null);
    }
    
    /// <summary>
    /// Создать прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="isSystem">Системная.</param>
    /// <param name="value">Числовое значение.</param>
    [Public, Remote]
    public virtual void CreateConstant(string name, bool isSystem, Double value)
    {
      CreateConstant(name, isSystem, null, value, null, null);
    }
    
    /// <summary>
    /// Создать прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="isSystem">Системная.</param>
    /// <param name="value">Дата.</param>
    [Public, Remote]
    public virtual void CreateConstant(string name, bool isSystem, DateTime value)
    {
      CreateConstant(name, isSystem, null, null, value, null);
    }
    
    /// <summary>
    /// Создать прикладную константу.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <param name="isSystem">Системная.</param>
    /// <param name="value">Строковое значение.</param>
    /// <param name="isPassword">Является паролем.</param>
    [Public, Remote]
    public virtual void CreateConstant(string name, bool isSystem, string value, bool isPassword)
    {
      CreateConstant(name, isSystem, null, null, null, value);
    }
    
    /// <summary>
    /// Создать прикладную константу.
    /// </summary>
    /// <param name="name">Имя коснтанты.</param>
    /// <param name="isSystem">Системная.</param>
    /// <param name="stringValue">Строковое значение.</param>
    /// <param name="number">Числовое значение.</param>
    /// <param name="date">Дата.</param>
    /// <param name="password">Является паролем.</param>
    public virtual void CreateConstant (string name, bool isSystem, string stringValue, Double? number, DateTime? date, string password)
    {
      var newConstant = ConstantsSettings.Create();
      newConstant.Name = name;
      newConstant.IsSystem = isSystem;
      if (stringValue != null)
      {
        newConstant.StringValue = stringValue;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.StringType;
      }
      
      if (number != null)
      {
        newConstant.NumberValue = number;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.NumberType;
      }
      
      if (date != null)
      {
        newConstant.DateTimeValue = date;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.DateTimeType;
      }
      
      if (password != null)
      {
        newConstant.PasswordValue = password;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.PasswordType;
      }
      
      newConstant.Save();
    }
    
    /// <summary>
    /// Получить значение константы.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <returns>Валидное строковое значение.</returns>
    [Public, Remote]
    public virtual string GetValue(string name)
    {
      var stringValue = ConstantsSettings.GetAll().FirstOrDefault(p => p.Name == name).Value;
      return stringValue;
    }
    
    /// <summary>
    /// Получить значение константы.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <returns>Целочисленное значение константы.</returns>
    [Public, Remote]
    public virtual int GetIntValue(string name)
    {
      var doubleValue = ConstantsSettings.GetAll().FirstOrDefault(p => p.Name == name).NumberValue;
      return Convert.ToInt32(doubleValue);
    }
    
    /// <summary>
    /// Получить значение константы.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <returns>Вещественное значение константы.</returns>
    [Public, Remote]
    public virtual double GetDoubleValue(string name)
    {
      return ConstantsSettings.GetAll().FirstOrDefault(p => p.Name == name).NumberValue.Value;
    }
    
    /// <summary>
    /// Получить значение константы.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <returns>Значение даты.</returns>
    [Public, Remote]
    public virtual DateTime GetDateTimeValue(string name)
    {
      return ConstantsSettings.GetAll().FirstOrDefault(p => p.Name == name).DateTimeValue.Value;
    }
  }
}