using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsGroup;

namespace DirRX.AppliedConstants
{
  partial class ConstantsGroupServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.IsSystem == true && string.IsNullOrEmpty(_obj.Guid))
        e.AddError(DirRX.AppliedConstants.ConstantsGroups.Resources.GuidFieldIsEmptyError);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsSystem = false;
    }
  }

}