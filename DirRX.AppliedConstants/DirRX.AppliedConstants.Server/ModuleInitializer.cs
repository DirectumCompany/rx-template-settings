using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace DirRX.AppliedConstants.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateConstantsGroups();
      CreateConstants();
    }
    
    /// <summary>
    /// Создание системных групп констант в инициализации.
    /// </summary>
    public virtual void CreateConstantsGroups()
    {
      
    }
    
    /// <summary>
    /// Создание системных прикладных констант в инициализации.
    /// </summary>
    public virtual void CreateConstants()
    {
      
    }
  }
}
