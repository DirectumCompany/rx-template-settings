using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsGroup;

namespace DirRX.AppliedConstants.Server
{
  partial class ConstantsGroupFunctions
  {
    /// <summary>
    /// Получить список дублирующихся записей.
    /// </summary>
    /// <returns>Список дублей.</returns>
    /// <remarks>Дубли находятся по полю Guid.</remarks>
    [Public, Remote(IsPure = true)]
    public virtual IQueryable<IConstantsGroup> GetDuplicates()
    {
      return ConstantsGroups.GetAll().Where(x => x.Id != _obj.Id && x.Guid == _obj.Guid);
    }
  }
}