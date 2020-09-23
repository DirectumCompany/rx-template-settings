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
    public virtual void CreateConstant(string name, bool isSystem, string value)
    {
      if (ConstantsSettings.GetAll(p => p.Name == name).Any())
        return;
      
      var newConstant = ConstantsSettings.Create();
      newConstant.Name = name;
      newConstant.IsSystem = isSystem;
      if (value != null)
      {
        newConstant.StringValue = value;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.StringType;
      }
      newConstant.Save();
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
      if (ConstantsSettings.GetAll(p => p.Name == name).Any())
        return;
      
      var newConstant = ConstantsSettings.Create();
      newConstant.Name = name;
      newConstant.IsSystem = isSystem;
      if (value != null)
      {
        newConstant.NumberValue = value;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.NumberType;
      }
      newConstant.Save();
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
      if (ConstantsSettings.GetAll(p => p.Name == name).Any())
        return;
      
      var newConstant = ConstantsSettings.Create();
      newConstant.Name = name;
      newConstant.IsSystem = isSystem;
      if (value != null)
      {
        newConstant.DateTimeValue = value;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.DateTimeType;
      }
      newConstant.Save();
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
      if (ConstantsSettings.GetAll(p => p.Name == name).Any())
        return;
      
      var newConstant = ConstantsSettings.Create();
      newConstant.Name = name;
      newConstant.IsSystem = isSystem;
      if (value != null)
      {
        newConstant.PasswordValue = value;
        newConstant.Type = AppliedConstants.ConstantsSetting.Type.PasswordType;
      }
      newConstant.Save();
    }
    
    
    /// <summary>
    /// Получить значение константы.
    /// </summary>
    /// <param name="name">Имя константы.</param>
    /// <returns>Значение константы.</returns>
    [Public]
    public virtual dynamic GetValue(string name)
    {
      var constant = ConstantsSettings.GetAll().FirstOrDefault(p => p.Name == name);
      if (constant != null)
      {
        if (constant.Type == AppliedConstants.ConstantsSetting.Type.StringType)
          return constant.StringValue;
        
        if (constant.Type == AppliedConstants.ConstantsSetting.Type.NumberType)
          return constant.NumberValue;
        
        if (constant.Type == AppliedConstants.ConstantsSetting.Type.DateTimeType)
          return constant.DateTimeValue.Value;
        
        if (constant.Type == AppliedConstants.ConstantsSetting.Type.PasswordType)
          return constant.PasswordValue;
        return constant.Value;
      }
      
      return null;
    }
  }
}