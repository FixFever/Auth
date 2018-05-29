using System;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;

namespace Auth.Core.Services
{
	public abstract class BaseService
	{
		protected readonly IAuthRepository AuthRepository;
		protected readonly ITokenController TokenController;
		protected readonly AuthOptions AuthOptions;
		protected readonly ILogRepository LogRepository;

		public BaseService(IAuthRepository authRepository, ITokenController tokenController,
			ILogRepository logRepository, IOptions<AuthOptions> authOptions)
		{
			AuthRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
			TokenController = tokenController ?? throw new ArgumentNullException(nameof(tokenController));
			LogRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
			AuthOptions = authOptions.Value;
		}

		/// <summary>
		/// Выполнить операцию с обработкой исключений
		/// </summary>
		protected Result<T> DoWithExceptionHandling<T>(int? userId, object data, Func<Result<T>> action, [CallerMemberName] string methodName = null)
		{
			try
			{
				// Выполняем действие
				var result = action();
				if (!result.IsSuccess)
					LogRepository.LogError(userId, result.Error, data, methodName);

				return result;
			}
			catch (AuthException ex)
			{
				LogRepository.LogError(userId, ex.ErrorCodeWithDescription, data, methodName);
				return Result<T>.Fail(ex.ErrorCodeWithDescription);
			}
			catch (SqlException ex)
			{
				LogRepository.LogException(userId, ex, data, methodName);
				return Result<T>.Fail(Errors.ConnectToDbError);
			}
			catch (Exception ex)
			{
				LogRepository.LogException(userId, ex, data, methodName);
				return Result<T>.Fail("Необработанное исключение", ex);
			}
		}
	}
}
