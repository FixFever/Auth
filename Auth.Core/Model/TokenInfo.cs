using System;

namespace Auth.Core.Model
{
	/// <summary>
	/// Класс для работы с информацией о токене
	/// </summary>
	public class TokenInfo
	{
		public TokenInfo(int userId, TimeSpan timeToLife)
		{
			Token = Guid.NewGuid();
			UserId = userId;
			CreationDate = DateTime.Now;
			_timeToLife = timeToLife;
			DeathDate = CreationDate + _timeToLife;
		}

		private readonly TimeSpan _timeToLife;

		/// <summary>
		/// Токен
		/// </summary>
		public Guid Token { get; set; }

		/// <summary>
		/// Id пользователя
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Дата создания
		/// </summary>
		public DateTime CreationDate { get; set; }

		/// <summary>
		/// Дата и время смерти токена
		/// </summary>
		public DateTime DeathDate { get; set; }

		/// <summary>
		/// Актуальность токена
		/// </summary>
		public bool isActual
		{
			get { return DateTime.Now < DeathDate; }
		}

		/// <summary>
		/// Продление жизни токена, если он актуален
		/// </summary>
		public void ExtendLife()
		{
			// Мертвецов не оживляем
			if (!isActual)
				return;

			DeathDate = DateTime.Now + _timeToLife;
		}
	}
}
