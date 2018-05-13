using System.Collections.Generic;

namespace Auth.Model
{
	public static class Errors
    {
		// Отсутствие входных данных
		public static readonly KeyValuePair<string, string> RequestIsEmpty = new KeyValuePair<string, string>("Auth_0000", "Отсутствуют данные запроса");
		public static readonly KeyValuePair<string, string> LoginIsEmpty = new KeyValuePair<string, string>("Auth_0001", "Логин не указан");
		public static readonly KeyValuePair<string, string> PasswordIsEmpty = new KeyValuePair<string, string>("Auth_0002", "Пароль не указан");

		// Неправильность входных данных
		public static readonly KeyValuePair<string, string> WrongLoginOrPassword = new KeyValuePair<string, string>("Auth_0003", "Указан неверный логин или пароль");

		// Системные ошибки
		public static readonly KeyValuePair<string, string> ConnectToDbError = new KeyValuePair<string, string>("Auth_0005", "Ошибка подключения к БД");
	}
}
