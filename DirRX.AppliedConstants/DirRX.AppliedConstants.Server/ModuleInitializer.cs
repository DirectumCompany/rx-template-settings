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
      CreateRoles();
      GrantRightsOnDatabooks();
      CreateConstants();
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create CustomConstants Roles");      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(DirRX.AppliedConstants.Resources.AppliedConstantManagerRoleName,
                                                                      DirRX.AppliedConstants.Resources.AppliedConstantManagerRoleDescription,
                                                                      Constants.Module.ManagerRoleGuid);
    }
    
    /// <summary>
    /// Получить роль менеджера прикладных констант.
    /// </summary>
    /// <returns>Роль менеджера прикладных констант.</returns>
    public static IRole GetAppliedConstantsManagerRole()
    {
      return Roles.GetAll().FirstOrDefault(r => r.Sid == Constants.Module.ManagerRoleGuid);
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      var managers = GetAppliedConstantsManagerRole();
      ConstantsSettings.AccessRights.Grant(managers, DefaultAccessRightsTypes.Change);
        
    }
    
    public virtual void CreateConstants()
    {
      Functions.Module.CreateConstant("TestConstant", false, 6432);
    }
  }
}
