using System;
using Auth.Core.Model;

namespace Auth.Core.Interfaces
{
	/// <summary>
	/// Сервис с действиями, доступным обычным пользователям
	/// </summary>
	public interface IUserService
	{
		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		Result<UserDto[]> GetAllUsers(Guid token);

		/// <summary>
		/// Возвращает данные о пользователе по его id
		/// </summary>
		Result<UserDto> GetUser(int userId, Guid token);
	}
}
