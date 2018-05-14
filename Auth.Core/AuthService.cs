using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;

namespace Auth.Core
{
	/// <summary>
	/// Сервис Auth
	/// </summary>
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly ITokenController _tokenController;
		private readonly ILogRepository _logRepository;
		private readonly AuthOptions _authOptions;

		public AuthService(IAuthRepository authRepository, ITokenController tokenController,
			ILogRepository logRepository, IOptions<AuthOptions> authOptions)
		{
			_authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
			_tokenController = tokenController ?? throw new ArgumentNullException(nameof(tokenController));
			_logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
			_authOptions = authOptions.Value;
		}

		/// <summary>
		/// Возвращает информацию о Пользователе, если удалось пройти аутентификацию по логину и паролю
		/// </summary>
		public Result<Guid?> Login(string login, string password)
		{
			return DoWithExceptionHandling(null, login, () =>
		   {
			   if (string.IsNullOrWhiteSpace(login))
				   return Result<Guid?>.Fail(Errors.LoginIsEmpty);

			   if (string.IsNullOrWhiteSpace(password))
				   return Result<Guid?>.Fail(Errors.PasswordIsEmpty);

			   // Если логинится рут-пользователь, то возвращаем токен из конфига
			   if (login == _authOptions.RootLogin && password == _authOptions.RootPassword)
				   return Result<Guid?>.Success(_authOptions.RootToken);

			   var authenticateResult = _authRepository.Authenticate(login, password);

			   if (!authenticateResult.IsSuccess)
				   return Result<Guid?>.Fail(authenticateResult.Error);

			   var token = _tokenController.CreateOrUpdateToken(authenticateResult.Data.Id);

			   return Result<Guid?>.Success(token);
		   });
		}

		/// <summary>
		/// Добавить нового пользователя в хранилище
		/// </summary>
		public Result<bool> CreateUser(UserRequestDto user, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, null, user, () =>
			{
				if (user == null || string.IsNullOrEmpty(user.Login))
					return Result<bool>.Fail(Errors.RequestIsEmpty);

				return _authRepository.CreateUser(user);
			});
		}

		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		public Result<UserDto[]> GetAllUsers(Guid token)
		{
			return DoWithCheckTokenIsActual(token, null, _authRepository.GetAllUsers);
		}

		/// <summary>
		/// Возвращает данные о пользователе по его id
		/// </summary>
		public Result<UserDto> GetUser(int userId, Guid token)
		{
			return DoWithCheckTokenIsActual(token, userId, () => _authRepository.GetUserById(userId));
		}

		/// <summary>
		/// Блокирует/разблокирует пользователя
		/// </summary>
		public Result<bool> SetUserIsActive(int userId, bool IsActive, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, IsActive, () =>
			{
				var result = _authRepository.SetUserIsActive(userId, IsActive);

				// Если блокируем пользователя, то удаляем его токен
				if (IsActive == false && result.IsSuccess)
				{
					var userToken = _tokenController.GetTokenForUser(userId);
					if (userToken.HasValue)
						_tokenController.RemoveToken(userToken.Value);
				}

				return result;
			});
		}

		/// <summary>
		/// Назначает/сбрасывает права админа
		/// </summary>
		public Result<bool> SetUserIsAdmin(int userId, bool IsAdmin, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, IsAdmin, () => _authRepository.SetUserIsAdmin(userId, IsAdmin));
		}

		/// <summary>
		/// Сменить пароль пользователя
		/// </summary>
		public Result<bool> SetUserPassword(int userId, string password, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, null, () =>
			{
				var result = _authRepository.SetUserPassword(userId, password);

				// При успешной смене пароля сбрасываем токен пользователя
				if (result.IsSuccess)
				{
					var userToken = _tokenController.GetTokenForUser(userId);
					if (userToken.HasValue)
						_tokenController.RemoveToken(userToken.Value);
				};

				return result;
			});
		}

		/// <summary>
		/// Обновляет инфу о пользователе в базе данных
		/// </summary>
		public Result<bool> UpdateUser(int userId, UserRequestDto user, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, user, () =>
			{
				if (user == null || string.IsNullOrEmpty(user.Login))
					return Result<bool>.Fail(Errors.RequestIsEmpty);

				return _authRepository.UpdateUser(userId, user);
			});
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
				if (token == _authOptions.RootToken)
					return Result<int>.Success(0);

				var tokenIsActual = _tokenController.CheckToken(token);

				if (!tokenIsActual)
					return Result<int>.Fail(Errors.TokenIsNonActual);

				return Result<int>.Success(_tokenController.GetUserId(token));
			}, methodName);

			if (!checkTokenResult.IsSuccess)
				return Result<T>.Fail(checkTokenResult.Error);

			var userId = checkTokenResult.Data;

			// Выполняем действие
			return DoWithExceptionHandling(userId, data, action, methodName);
		}

		/// <summary>
		/// Выполнить операцию с проверкой админских прав
		/// </summary>
		private Result<T> DoWithCheckAdminPermissionAndLog<T>(Guid adminToken, int? userId, object data, Func<Result<T>> action, [CallerMemberName] string methodName = null)
		{
			var checkTokenResult = DoWithExceptionHandling(null, adminToken, () =>
			{
				if (adminToken == Guid.Empty)
					return Result<int>.Fail(Errors.TokenIsEmpty);

				// Для токена рут-пользователя проверка не нужна
				if (adminToken == _authOptions.RootToken)
					return Result<int>.Success(0);

				var adminId = _tokenController.GetUserId(adminToken);

				var getUserResult = _authRepository.GetUserById(adminId);

				if (!getUserResult.IsSuccess)
					return Result<int>.Fail(getUserResult.Error);

				var adminUser = getUserResult.Data;

				if (!adminUser.IsAdmin)
					return Result<int>.Fail(Errors.AccessDenied);

				// Выполняем действие
				return Result<int>.Success(adminId);
			}, methodName);

			if (!checkTokenResult.IsSuccess)
				return Result<T>.Fail(checkTokenResult.Error);

			int adminUserId = checkTokenResult.Data;

			// Выполняем действие
			return DoWithExceptionHandling(adminUserId, data, () =>
			{
				var actionResult = action();

				if (actionResult.IsSuccess)
					_logRepository.LogAdminAction(adminUserId, userId, data, methodName);

				return actionResult;
			}, methodName);
		}
		
		/// <summary>
		/// Выполнить операцию с обработкой исключений
		/// </summary>
		private Result<T> DoWithExceptionHandling<T>(int? userId, object data, Func<Result<T>> action, [CallerMemberName] string methodName = null)
		{
			try
			{
				// Выполняем действие
				var result = action();
				if (!result.IsSuccess)
					_logRepository.LogError(userId, result.Error, data, methodName);

				return result;
			}
			catch (AuthException ex)
			{
				_logRepository.LogError(userId, ex.ErrorCodeWithDescription, data, methodName);
				return Result<T>.Fail(ex.ErrorCodeWithDescription);
			}
			catch (SqlException ex)
			{
				_logRepository.LogException(userId, ex, data, methodName);
				return Result<T>.Fail(Errors.ConnectToDbError);
			}
			catch (Exception ex)
			{
				_logRepository.LogException(userId, ex, data, methodName);
				return Result<T>.Fail("Необработанное исключение", ex);
			}
		}
	}
}
