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
      _obj.PasswordTemp = string.Empty;
      _obj.IsSystem = false;
      _obj.BoolValue = false;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверка: для системных констант обязательно должен быть заполнен GUID.
      if (_obj.IsSystem == true && string.IsNullOrEmpty(_obj.Guid))
        e.AddError(DirRX.AppliedConstants.ConstantsSettings.Resources.GuidFieldIsEmptyError);
      
      // Проверка: значение поля GUID должно быть в корректном формате.
      var constantGuid = Guid.Empty;
      if (!string.IsNullOrEmpty(_obj.Guid) && !Guid.TryParse(_obj.Guid, out constantGuid))
        e.AddError(DirRX.AppliedConstants.Resources.GuidNotValid);
      
      // Проверка: для системных констант обязательно должна быть указана группа констант.
      if (_obj.IsSystem == true && _obj.ConstantsGroup == null)
        e.AddError(DirRX.AppliedConstants.ConstantsSettings.Resources.ConstantsGroupIsEmptyError);
      
      // Проверка: обнаружение дубликатов настроек констант.
      if (Functions.ConstantsSetting.GetDuplicates(_obj).Any())
        e.AddError(DirRX.AppliedConstants.Resources.DuplicatesDetected, _obj.Info.Actions.ShowDuplicates);
      
      // Заполнение отображаемого значения константы.
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.StringType)
        _obj.PublicValue = _obj.Value = _obj.StringValue;
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.IntegerType)
        _obj.PublicValue = _obj.Value = _obj.IntegerValue.ToString();
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.DoubleType)
        _obj.PublicValue = _obj.Value = _obj.DoubleValue.ToString();
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.IdentifierType)
        _obj.PublicValue = _obj.Value = _obj.IdentifierValue.ToString();
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.DateType && _obj.DateValue.HasValue)
        _obj.PublicValue = _obj.Value = _obj.DateValue.Value.ToShortDateString();
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.BoolType && _obj.BoolValue.HasValue)
      {
        _obj.Value = _obj.BoolValue.ToString();
        _obj.PublicValue = _obj.BoolValue == true ?
          DirRX.AppliedConstants.ConstantsSettings.Resources.TrueLocalizedValue :
          DirRX.AppliedConstants.ConstantsSettings.Resources.FalseLocalizedValue;
      }
      
      if (_obj.Type == AppliedConstants.ConstantsSetting.Type.PasswordType && !string.IsNullOrEmpty(_obj.PasswordTemp))
        Functions.ConstantsSetting.EncryptPasswordValue(_obj);
    }
  }
}