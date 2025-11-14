using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsSetting;

namespace DirRX.AppliedConstants.Shared
{
  partial class ConstantsSettingFunctions
  {
    /// <summary>
    /// Определить состояния полей в карточке.
    /// </summary>
    public virtual void SetPropertiesState()
    {
      // Настройка видимости свойств.
      _obj.State.Properties.StringValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.StringType;
      _obj.State.Properties.IntegerValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.IntegerType;
      _obj.State.Properties.DoubleValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.DoubleType;
      _obj.State.Properties.DateValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.DateType;
      _obj.State.Properties.PasswordValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.PasswordType;
      _obj.State.Properties.IdentifierValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.IdentifierType;
      _obj.State.Properties.BoolValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.BoolType;

      // Настройка доступности свойств.
      var isSystem = _obj.IsSystem == true;
      _obj.State.Properties.Type.IsEnabled = !isSystem;
      _obj.State.Properties.Name.IsEnabled = !isSystem;
      _obj.State.Properties.Guid.IsEnabled = !isSystem;
      _obj.State.Properties.ConstantsGroup.IsEnabled = !isSystem;
    }
  }
}