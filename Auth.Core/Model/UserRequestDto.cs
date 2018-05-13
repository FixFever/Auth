using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	/// <summary>
	/// Dto запроса на создание/обновление пользователя
	/// </summary>
	[DataContract]
	public class UserRequestDto
    {
		/// <summary>
		/// Логин пользователя
		/// </summary>
		[DataMember]
		public string Login { get; set; }

		/// <summary>
		/// ФИО пользователя
		/// </summary>
		[DataMember]
		public string FullName { get; set; }
	}
}
