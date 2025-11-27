using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsSetting;

namespace DirRX.AppliedConstants
{
  partial class ConstantsSettingSharedHandlers
  {
    public virtual void PasswordValueChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Если в пароле указана специальная строка из константы HidePasswordValue, то указанное значение не является паролем.
      if (e.NewValue != e.OldValue && e.NewValue != Constants.ConstantsSetting.HidePasswordValue)
      {
        // Если новое значение - пустая строка, очищаем все поля пароля.
        // Иначе сохраняем новый пароль как временный на время сессии редактирования.
        // При сохранении прикладной константы он будет зашифрован и записан в специальное поле.
        if (string.IsNullOrEmpty(e.NewValue))
          Functions.ConstantsSetting.ClearPasswordValue(_obj);
        else
          _obj.PasswordTemp = e.NewValue;
      }
    }

    public virtual void TypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      // Очистка значения константы при смене её типа.
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue != ConstantsSetting.Type.BoolType && _obj.BoolValue != false)
          _obj.BoolValue = false;
        if (e.NewValue != ConstantsSetting.Type.StringType && _obj.StringValue != null)
          _obj.StringValue = string.Empty;
        if (e.NewValue != ConstantsSetting.Type.DateType && _obj.DateValue != null)
          _obj.DateValue = null;
        if (e.NewValue != ConstantsSetting.Type.DoubleType && _obj.DoubleValue != null)
          _obj.DoubleValue = null;
        if (e.NewValue != ConstantsSetting.Type.IdentifierType && _obj.IdentifierValue != null)
          _obj.IdentifierValue = null;
        if (e.NewValue != ConstantsSetting.Type.IntegerType && _obj.IntegerValue != null)
          _obj.IntegerValue = null;
        if (e.NewValue != ConstantsSetting.Type.PasswordType && _obj.PasswordValue != null)
          Functions.ConstantsSetting.ClearPasswordValue(_obj);
      }
      
      Functions.ConstantsSetting.SetPropertiesState(_obj);
    }
  }
}