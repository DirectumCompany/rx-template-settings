using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsGroup;

namespace DirRX.AppliedConstants
{
  partial class ConstantsGroupClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.ConstantsGroup.SetPropertiesState(_obj);
    }

  }
}