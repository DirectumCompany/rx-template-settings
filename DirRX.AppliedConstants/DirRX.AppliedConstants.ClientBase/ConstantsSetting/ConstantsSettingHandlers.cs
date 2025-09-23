using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsSetting;

namespace DirRX.AppliedConstants
{
  partial class ConstantsSettingClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.ConstantsSetting.SetPropertiesState(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      Functions.ConstantsSetting.SetPropertiesState(_obj);
      
      _obj.PasswordTemp = _obj.Value;
    }
  }
}