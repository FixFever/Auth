using Auth.Core.Model;
using System;

namespace Auth.Core.Interfaces
{
	/// <summary>
	/// Сервис для действий администраторов
	/// </summary>
	public interface IAdminService
	{
		/// <summary>
		/// Добавить нового пользователя в хранилище
		/// </summary>
		Result<UserDto> CreateUser(UserRequestDto user, Guid token);

		/// <summary>
		/// Сменить пароль пользователя
		/// </summary>
		Result<bool> SetUserPassword(int userId, string password, Guid token);

		/// <summary>
		/// Обновляет инфу о пользователе в базе данных
		/// </summary>
		Result<bool> UpdateUser(int userId, UserRequestDto user, Guid token);

		/// <summary>
		/// Назначает/сбрасывает права админа
		/// </summary>
		Result<bool> SetUserIsAdmin(int userId, bool IsAdmin, Guid token);

		/// <summary>
		/// Блокирует/разблокирует пользователя
		/// </summary>
		Result<bool> SetUserIsActive(int userId, bool IsActive, Guid token);
	}
}
