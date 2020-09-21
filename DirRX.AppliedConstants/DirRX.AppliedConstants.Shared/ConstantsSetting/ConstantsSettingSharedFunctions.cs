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
      _obj.State.Properties.StringValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.StringType;
      _obj.State.Properties.NumberValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.NumberType;
      _obj.State.Properties.DateTimeValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.DateTimeType;
      _obj.State.Properties.PasswordValue.IsVisible = _obj.Type == AppliedConstants.ConstantsSetting.Type.PasswordType;
      
      var isSystem = _obj.IsSystem == true;
      _obj.State.Properties.Type.IsEnabled = !isSystem;
      _obj.State.Properties.StringValue.IsEnabled = !isSystem;
      _obj.State.Properties.NumberValue.IsEnabled = !isSystem;
      _obj.State.Properties.DateTimeValue.IsEnabled = !isSystem;
      _obj.State.Properties.PasswordValue.IsEnabled = !isSystem;
    }
  }
}