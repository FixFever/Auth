using System;
using Auth.Core.Model;

namespace Auth.Core.Interfaces
{
	/// <summary>
	/// Сервис для аутентификации
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Возвращает информацию о Пользователе, если удалось пройти аутентификацию по логину и паролю
		/// </summary>
		Result<Guid?> Login(string login, string password);
	}
}
