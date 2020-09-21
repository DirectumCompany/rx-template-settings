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
      Functions.ConstantsSetting.SetPropertiesState(_obj);
    }
  }
}