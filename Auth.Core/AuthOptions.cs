using System;

namespace Auth.Core
{
	/// <summary>
	/// Настройки сервиса Auth
	/// </summary>
	public class AuthOptions
	{
		/// <summary>
		/// Время жизни токена
		/// </summary>
		public TimeSpan TokenTimeToLife { get; set; }

		/// <summary>
		/// Логин рута
		/// </summary>
		public string RootLogin { get; set; }

		/// <summary>
		/// Пароль рута
		/// </summary>
		public string RootPassword { get; set; }

		/// <summary>
		/// Токен рута
		/// </summary>
		public Guid RootToken { get; set; }
	}
}
