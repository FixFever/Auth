using Auth.Core.Model;

namespace Auth.Core.Interfaces
{
	public interface IAuthRepository
	{
		/// <summary>
		/// Возвращает информацию о Пользователе, если удалось пройти аутентификацию по логину и паролю
		/// </summary>
		Result<UserDto> Authenticate(string login, string password);

		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		Result<UserDto[]> GetAllUsers();

		/// <summary>
		/// Добавить нового пользователя в хранилище
		/// </summary>
		Result<bool> CreateUser(UserRequestDto user);

		/// <summary>
		/// Сменить пароль пользователя
		/// </summary>
		Result<bool> SetUserPassword(int userId, string password);

		/// <summary>
		/// Обновляет инфу о пользователе в базе данных
		/// </summary>
		Result<bool> UpdateUser(int userId, UserRequestDto user);

		/// <summary>
		/// Назначает/сбрасывает права админа
		/// </summary>
		Result<bool> SetUserIsAdmin(int userId, bool IsAdmin);
		
		/// <summary>
		/// Блокирует/разблокирует пользователя
		/// </summary>
		Result<bool> SetUserIsActive(int userId, bool IsActive);

		/// <summary>
		/// Получить данные о пользователе по его id
		/// </summary>
		Result<UserDto> GetUserById(int userId);
	}
}
