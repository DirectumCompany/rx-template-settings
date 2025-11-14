using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsGroup;

namespace DirRX.AppliedConstants.Shared
{
  partial class ConstantsGroupFunctions
  {
    /// <summary>
    /// Определить состояния полей в карточке.
    /// </summary>
    public virtual void SetPropertiesState()
    {
      _obj.State.IsEnabled = _obj.IsSystem != true;
    }
  }
}