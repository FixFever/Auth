using System.Collections.Generic;

namespace Auth.Core
{
	public static class Errors
    {
		public static readonly KeyValuePair<string, string> AccessDenied = new KeyValuePair<string, string>("Auth_0403", "Недостаточно прав для выполнения действия");

		// Отсутствие входных данных
		public static readonly KeyValuePair<string, string> RequestIsEmpty = new KeyValuePair<string, string>("Auth_0000", "Отсутствуют данные запроса");
		public static readonly KeyValuePair<string, string> LoginIsEmpty = new KeyValuePair<string, string>("Auth_0001", "Логин не указан");
		public static readonly KeyValuePair<string, string> PasswordIsEmpty = new KeyValuePair<string, string>("Auth_0002", "Пароль не указан");

		// Неправильность входных данных
		public static readonly KeyValuePair<string, string> WrongLoginOrPassword = new KeyValuePair<string, string>("Auth_0003", "Указан неверный логин или пароль");
		public static readonly KeyValuePair<string, string> UserIsNonActive = new KeyValuePair<string, string>("Auth_0004", "Пользователь заблокирован");
		public static readonly KeyValuePair<string, string> UserAlreadyExists = new KeyValuePair<string, string>("Auth_0005", "Пользователь c таким логином уже существует");
		public static readonly KeyValuePair<string, string> UserNotFound = new KeyValuePair<string, string>("Auth_0006", "Пользователь не найден");
		
		// Токены
		public static readonly KeyValuePair<string, string> TokenIsEmpty = new KeyValuePair<string, string>("Auth_0007", "Токен не указан");
		public static readonly KeyValuePair<string, string> TokenIsNonActual = new KeyValuePair<string, string>("Auth_0008", "Токен не актуален или не существует");

		// Системные ошибки
		public static readonly KeyValuePair<string, string> ConnectToDbError = new KeyValuePair<string, string>("Auth_0006", "Ошибка подключения к БД");

		public static readonly KeyValuePair<string, string> UnexpectedError = new KeyValuePair<string, string>("Auth_0404", "Неизвестная ошибка");
	}
}
