using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	/// <summary>
	/// Результат выполнения операции
	/// </summary>
	[DataContract]
	public class Result<T>
	{
		/// <summary>
		/// Результат успешной операции
		/// </summary>
		[DataMember]
		public T Data { get; set; }

		/// <summary>
		/// Список ошибок (коды + описания)
		/// </summary>
		[DataMember]
		public KeyValuePair<string, string> Error { get; set; }

		/// <summary>
		/// Успешный результат
		/// </summary>
		[DataMember]
		public bool IsSuccess
		{
			get
			{
				return string.IsNullOrWhiteSpace(Error.Key);
			}
		}

		/// <summary>
		/// Создаёт результат успешной работы
		/// </summary>
		public static Result<T> Success(T data)
		{
			return new Result<T>
			{
				Data = data
			};
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		public static Result<T> Fail(KeyValuePair<string, string> error) => new Result<T>
		{
			Error = error
		};

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		/// <param name="prefix">Текст, который будет добавляться в описание ошибки</param>
		/// <param name="exception"></param>
		public static Result<T> Fail(string prefix, Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			string errorMessage = exception.Message + ": " + exception.StackTrace; ;

			if (!string.IsNullOrWhiteSpace(prefix))
				errorMessage = prefix + ": " + errorMessage;

			return new Result<T>
			{
				Error = new KeyValuePair<string, string>(exception.GetType().Name, errorMessage)
			};
		}
	}
}
