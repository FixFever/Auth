using System;
using System.Runtime.CompilerServices;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;

namespace Auth.Core.Services
{
	/// <summary>
	/// Сервис с действиями, доступным обычным пользователям
	/// </summary>
	public class UserService : BaseService, IUserService
	{
		/// <inheritdoc />
		public UserService(IAuthRepository authRepository, ITokenController tokenController, ILogRepository logRepository, IOptions<AuthOptions> authOptions)
			: base(authRepository, tokenController, logRepository, authOptions)
		{
		}

		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		public Result<UserDto[]> GetAllUsers(Guid token)
		{
			return DoWithCheckTokenIsActual(token, null, AuthRepository.GetAllUsers);
		}

		/// <summary>
		/// Возвращает данные о пользователе по его id
		/// </summary>
		public Result<UserDto> GetUser(int userId, Guid token)
		{
			return DoWithCheckTokenIsActual(token, userId, () => AuthRepository.GetUserById(userId));
		}

		/// <summary>
		/// Выполнить операцию с проверкой актуальности токена
		/// </summary>
		private Result<T> DoWithCheckTokenIsActual<T>(Guid token, object data, Func<Result<T>> action, [CallerMemberName] string methodName = null)
		{
			var checkTokenResult = DoWithExceptionHandling(null, token, () =>
			{
				if (token == Guid.Empty)
					return Result<int>.Fail(Errors.TokenIsEmpty);

				// Для токена рут-пользователя проверка не нужна
				if (token == AuthOptions.RootToken)
					return Result<int>.Success(0);

				var tokenIsActual = TokenController.CheckToken(token);

				if (!tokenIsActual)
					return Result<int>.Fail(Errors.TokenIsNonActual);

				return Result<int>.Success(TokenController.GetUserId(token));
			}, methodName);

			if (!checkTokenResult.IsSuccess)
				return Result<T>.Fail(checkTokenResult.Error);

			var userId = checkTokenResult.Data;

			// Выполняем действие
			return DoWithExceptionHandling(userId, data, action, methodName);
		}
	}
}
