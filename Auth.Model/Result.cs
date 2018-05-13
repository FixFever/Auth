using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Auth.Model
{
	/// <summary>
	/// Результат выполнения операции
	/// </summary>
	[DataContract]
	public class Result
	{
		/// <summary>
		/// Успешный результат
		/// </summary>
		[DataMember]
		public bool IsSuccess
		{
			get
			{
				return Errors == null || Errors.Length == 0;
			}
		}

		/// <summary>
		/// Список ошибок (коды + описания)
		/// </summary>
		[DataMember]
		public KeyValuePair<string, string>[] Errors { get; set; }

		/// <summary>
		/// Создаёт результат успешной работы
		/// </summary>
		public static Result Success()
		{
			return new Result();
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		public static Result Error(KeyValuePair<string, string> error)
		{
			return new Result
			{
				Errors = new[] { error }
			};
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		public static Result Error(IEnumerable<KeyValuePair<string, string>> errors)
		{
			if (errors == null)
				throw new ArgumentNullException(nameof(errors));

			return new Result
			{
				Errors = errors.ToArray(),
			};
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		/// <param name="prefix">Текст, который будет добавляться в описание ошибки</param>
		/// <param name="exception"></param>
		public static Result Error(string prefix, Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			string errorMessage = exception.Message + ": " + exception.StackTrace; ;

			if (!string.IsNullOrWhiteSpace(prefix))
				errorMessage = prefix + ": " + errorMessage;

			return new Result
			{
				Errors = new[] { new KeyValuePair<string, string>(exception.GetType().Name, errorMessage) }
			};
		}
	}

	[DataContract]
	public class Result<T> : Result
	{
		/// <summary>
		/// Результат успешной операции
		/// </summary>
		[DataMember]
		public T Data { get; set; }
		
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
		public new static Result<T> Error(KeyValuePair<string, string> error)
		{
			return new Result<T>
			{
				Errors = new[] { error }
			};
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		public new static Result<T> Error(IEnumerable<KeyValuePair<string, string>> errors)
		{
			if (errors == null)
				throw new ArgumentNullException(nameof(errors));

			return new Result<T>
			{
				Errors = errors.ToArray(),
			};
		}

		/// <summary>
		/// Создаёт результат - ошибку
		/// </summary>
		/// <param name="prefix">Текст, который будет добавляться в описание ошибки</param>
		/// <param name="exception"></param>
		public new static Result<T> Error(string prefix, Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			string errorMessage = exception.Message + ": " + exception.StackTrace; ;

			if (!string.IsNullOrWhiteSpace(prefix))
				errorMessage = prefix + ": " + errorMessage;

			return new Result<T>
			{
				Errors = new[] { new KeyValuePair<string, string>(exception.GetType().Name, errorMessage) }
			};
		}
	}
}
