using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsSetting;

namespace DirRX.AppliedConstants
{

  partial class ConstantsSettingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.PasswordTemp = null;
      
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.StringType)
        _obj.PublicValue = _obj.Value = _obj.StringValue;
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.NumberType)
         _obj.PublicValue = _obj.Value = _obj.NumberValue.ToString();
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.DateTimeType)
         _obj.PublicValue = _obj.Value = _obj.DateTimeValue.Value.ToShortDateString();
      if (_obj.Type ==  AppliedConstants.ConstantsSetting.Type.PasswordType)
      {
        _obj.Value = _obj.PasswordTemp;
        _obj.PasswordTemp = string.Empty;
         _obj.PublicValue = _obj.PasswordValue = "*******";
      }
    }
  }
}