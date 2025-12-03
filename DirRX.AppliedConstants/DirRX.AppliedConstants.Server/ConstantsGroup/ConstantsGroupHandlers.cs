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
      // Проверка: для системных групп констант обязательно должен быть заполнен GUID.
      if (_obj.IsSystem == true && string.IsNullOrEmpty(_obj.Guid))
        e.AddError(DirRX.AppliedConstants.ConstantsGroups.Resources.GuidFieldIsEmptyError);
      
      // Проверка: обнаружение дубликатов настроек констант.
      if (Functions.ConstantsGroup.GetDuplicates(_obj).Any())
        e.AddError(DirRX.AppliedConstants.Resources.DuplicatesDetected, _obj.Info.Actions.ShowDuplicates);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsSystem = false;
    }
  }

}