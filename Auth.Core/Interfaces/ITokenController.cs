using System;

namespace Auth.Core.Interfaces
{
	public interface ITokenController
	{
		/// <summary>
		/// Создать новый токен или обновить существующий
		/// </summary>
		Guid CreateOrUpdateToken(int userId);

		/// <summary>
		/// Получить существующий токен пользователя, если он есть
		/// </summary>
		Guid? GetTokenForUser(int userId);

		/// <summary>
		/// Получить UserId по токену
		/// </summary>
		int GetUserId(Guid token);

		/// <summary>
		/// Удаляет токен
		/// </summary>
		void RemoveToken(Guid tokenId);

		/// <summary>
		/// Проверка актуальности токена
		/// Если проверка успешна - продлевается жизнь токена, иначе токен удаляется
		/// </summary>
		bool CheckToken(Guid tokenId);
	}
}
