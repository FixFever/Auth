using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	[DataContract]
	public class PasswordRequestDto
	{
		/// <summary>
		/// Пароль
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Password;
	}
}
