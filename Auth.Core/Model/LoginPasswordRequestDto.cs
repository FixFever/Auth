using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	/// <summary>
	/// Данные для аутентификации по логину/паролю
	/// </summary>
	[DataContract(Name = "AuthRequest")]
	public class LoginPasswordRequestDto
	{
		/// <summary>
		/// Логин
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Login;

		/// <summary>
		/// Пароль
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Password;
	}
}
