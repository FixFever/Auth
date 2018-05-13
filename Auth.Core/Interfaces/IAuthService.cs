using Auth.Core.Model;
using System;

namespace Auth.Core.Interfaces
{
	/// <summary>
	/// Сервис Auth
	/// </summary>
	public interface IAuthService
	{
		/// <summary>
		/// Возвращает информацию о Пользователе, если удалось пройти аутентификацию по логину и паролю
		/// </summary>
		Result<Guid?> Login(string login, string password);

		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		Result<UserDto[]> GetAllUsers(Guid token);

		/// <summary>
		/// Возвращает данные о пользователе по его id
		/// </summary>
		Result<UserDto> GetUser(int userId, Guid token);

		/// <summary>
		/// Добавить нового пользователя в хранилище
		/// </summary>
		Result<bool> CreateUser(UserRequestDto user, Guid token);

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
