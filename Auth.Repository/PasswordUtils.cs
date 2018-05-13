using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Repository
{
	/// <summary>
	/// Утилиты для работы с паролями
	/// </summary>
	internal static class PasswordUtils
	{
		/// <summary>
		/// Формирует хэш на основании пароля и соли
		/// </summary>
		public static string CreatePasswordHash(string password, string salt)
		{
			return Convert.ToBase64String(password.Hash(salt));
		}

		/// <summary>
		/// Генерирует случайный пароль
		/// </summary>
		public static string GeneratePassword()
		{
			return Guid.NewGuid().ToString().Take(6).ToString();
		}

		private static readonly Random Rand = new Random();

		private static byte[] Hash(this string password, string salt)
		{
			if (string.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));
			if (string.IsNullOrEmpty(salt))
				throw new ArgumentNullException(nameof(salt));

			var saltBytes = Encoding.ASCII.GetBytes("+auth+" + salt);

			using (var hasher = new SHA512Managed())
			{
				var passwordBytes = Encoding.ASCII.GetBytes(password).Concat(saltBytes).ToArray();
				return hasher.ComputeHash(passwordBytes);
			}
		}
	}
}