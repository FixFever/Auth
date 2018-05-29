using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Auth.Core
{
	/// <summary>
	/// Класс для управления токенами
	/// </summary>
	public class TokenController : ITokenController
	{
		/// <summary>
		/// Dictionary для хранения токенов в рантайме
		/// </summary>
		private static readonly ConcurrentDictionary<Guid, TokenInfo> Tokens = new ConcurrentDictionary<Guid, TokenInfo>();

		private readonly TimeSpan _timeToLife;

		public TokenController(IOptions<AuthOptions> authOptions)
		{
			_timeToLife = authOptions.Value.TokenTimeToLife;
		}

		/// <summary>
		/// Создать новый токен или обновить существующий
		/// </summary>
		public Guid CreateOrUpdateToken(int userId)
		{
			// Если есть актуальный токен для пользователя - возвращаем его
			var existedToken = GetTokenForUser(userId);
			if (existedToken.HasValue)
				return existedToken.Value;

			// Создаём новый токен
			var tokenInfo = new TokenInfo(userId, _timeToLife);
			Tokens.AddOrUpdate(tokenInfo.Token, tokenInfo, (key, oldToken) => tokenInfo);

			return tokenInfo.Token;
		}

		/// <summary>
		/// Получить существующий токен пользователя, если он есть
		/// </summary>
		public Guid? GetTokenForUser(int userId)
		{
			if (Tokens.IsEmpty || userId == default(int))
				return null;

			var tokeninfo = Tokens.Values.FirstOrDefault(t => t.UserId == userId);

			if (tokeninfo == null || !CheckToken(tokeninfo.Token))
				return null;

			return tokeninfo.Token;
		}

		/// <summary>
		/// Получить UserId по токену
		/// </summary>
		public int GetUserId(Guid token)
		{
			if (token == Guid.Empty)
				throw new AuthException(Errors.TokenIsEmpty);

			if (!CheckToken(token))
				throw new AuthException(Errors.TokenIsNonActual);

			return Tokens[token].UserId;
		}

		/// <summary>
		/// Проверка актуальности токена
		/// Если проверка успешна - продлевается жизнь токена, иначе токен удаляется
		/// </summary>
		public bool CheckToken(Guid tokenId)
		{
			TokenInfo token;
			var tokenExist = Tokens.TryGetValue(tokenId, out token);

			if (!tokenExist) // Токена не сущеcтвует
				return false;

			if (!token.isActual)
			{
				// Токен есть, но не актуален. Удаляем.
				TokenInfo value;
				Tokens.TryRemove(tokenId, out value);
				return false;
			}

			// Токен существует и актуален, продлеваем его жизнь
			token.ExtendLife();
			return true;
		}

		public void RemoveToken(Guid tokenId)
		{
			if (!Tokens.ContainsKey(tokenId))
				return;

			TokenInfo tInfo;
			Tokens.TryRemove(tokenId, out tInfo);
		}
	}
}
