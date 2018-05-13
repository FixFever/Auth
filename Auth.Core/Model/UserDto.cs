using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	/// <summary>
	/// Профиль пользователя
	/// </summary>
	[DataContract]
	public class UserDto : UserRequestDto
	{
		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Учётная запись активна
		/// </summary>
		[DataMember]
		public bool IsActive { get; set; }

		/// <summary>
		/// Учётная запись имеет права админа
		/// </summary>
		[DataMember]
		public bool IsAdmin { get; set; }
	}
}
