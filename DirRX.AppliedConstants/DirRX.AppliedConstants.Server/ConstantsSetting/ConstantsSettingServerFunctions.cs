using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.AppliedConstants.ConstantsSetting;

namespace DirRX.AppliedConstants.Server
{
  partial class ConstantsSettingFunctions
  {

    /// <summary>
    /// Получить список дублирующихся записей.
    /// </summary>
    /// <returns>Список дублей.</returns>
    /// <remarks>Дубли находятся по полю Guid.</remarks>
    [Public, Remote(IsPure = true)]
    public virtual IQueryable<IConstantsSetting> GetDuplicates()
    {
      return ConstantsSettings.GetAll().Where(x => x.Id != _obj.Id && x.Guid == _obj.Guid);
    }
    
    /// <summary>
    /// Зашифровать пароль.
    /// </summary>
    public void EncryptPasswordValue()
    {
      // 1. Временный пароль заполняется в событии изменения свойства PasswordValue, например, когда пользователь устанавливает пароль в карточке.
      // 2. Временный пароль шифруется.
      // 3. Зашифрованное значение записывается в прикладную константу.
      // 4. Видимые значения пароля в карточке и списке заменяются на специальную строку из константы HidePasswordValue.
      if (string.IsNullOrEmpty(_obj.PasswordTemp))
        return;
      
      _obj.Value = Encryption.Encrypt(_obj.PasswordTemp);
      _obj.PasswordTemp = string.Empty;
      _obj.PasswordValue = Constants.ConstantsSetting.HidePasswordValue;
      _obj.PublicValue = Constants.ConstantsSetting.HidePasswordValue;
    }

  }
}