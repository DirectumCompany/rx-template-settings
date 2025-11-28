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
      _obj.PasswordTemp = e.NewValue;
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
          _obj.PasswordValue = null;
      }
      
      Functions.ConstantsSetting.SetPropertiesState(_obj);
    }
  }
}