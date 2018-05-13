using System;
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
		private readonly AuthOptions _authOptions;

		public AuthService(IAuthRepository authRepository, ITokenController tokenController, IOptions<AuthOptions> authOptions)
		{
			_authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
			_tokenController = tokenController ?? throw new ArgumentNullException(nameof(tokenController));
			_authOptions = authOptions.Value;
		}

		/// <summary>
		/// Возвращает информацию о Пользователе, если удалось пройти аутентификацию по логину и паролю
		/// </summary>
		public Result<Guid?> Login(string login, string password)
		{
			return DoWithExceptionHandling(() =>
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
			return DoWithCheckAdminPermission(token, () => _authRepository.CreateUser(user));
		}

		/// <summary>
		/// Возвращает список всех пользователей
		/// </summary>
		public Result<UserDto[]> GetAllUsers(Guid token)
		{
			return DoWithCheckTokenIsActual(token, _authRepository.GetAllUsers);
		}

		/// <summary>
		/// Возвращает данные о пользователе по его id
		/// </summary>
		public Result<UserDto> GetUser(int userId, Guid token)
		{
			return DoWithCheckTokenIsActual(token, () => _authRepository.GetUserById(userId));
		}

		/// <summary>
		/// Блокирует/разблокирует пользователя
		/// </summary>
		public Result<bool> SetUserIsActive(int userId, bool IsActive, Guid token)
		{
			return DoWithCheckAdminPermission(token, () =>
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
			return DoWithCheckAdminPermission(token, () => _authRepository.SetUserIsAdmin(userId, IsAdmin));
		}

		/// <summary>
		/// Сменить пароль пользователя
		/// </summary>
		public Result<bool> SetUserPassword(int userId, string password, Guid token)
		{
			return DoWithCheckAdminPermission(token, () => _authRepository.SetUserPassword(userId, password));
		}

		/// <summary>
		/// Обновляет инфу о пользователе в базе данных
		/// </summary>
		public Result<bool> UpdateUser(int userId, UserRequestDto user, Guid token)
		{
			return DoWithCheckAdminPermission(token, () => _authRepository.UpdateUser(userId, user));
		}

		/// <summary>
		/// Выполнить операцию с проверкой актуальности токена
		/// </summary>
		private Result<T> DoWithCheckTokenIsActual<T>(Guid token, Func<Result<T>> action)
		{
			return DoWithExceptionHandling<T>(() =>
			{
				if (token == Guid.Empty)
					return Result<T>.Fail(Errors.TokenIsEmpty);

				// Для токена рут-пользователя проверка не нужна
				if (token == _authOptions.RootToken)
					return action();

				var tokenIsActual = _tokenController.CheckToken(token);

				if (!tokenIsActual)
					return Result<T>.Fail(Errors.TokenIsNonActual);

				// Выполняем действие
				return action();
			});
		}

		/// <summary>
		/// Выполнить операцию с проверкой админских прав
		/// </summary>
		private Result<T> DoWithCheckAdminPermission<T>(Guid token, Func<Result<T>> action)
		{
			return DoWithExceptionHandling<T>(() =>
			{
				if (token == Guid.Empty)
					return Result<T>.Fail(Errors.TokenIsEmpty);

				// Для токена рут-пользователя проверка не нужна
				if (token == _authOptions.RootToken)
					return action();

				var userId = _tokenController.GetUserId(token);

				var getUserResult = _authRepository.GetUserById(userId);

				if (!getUserResult.IsSuccess)
					return Result<T>.Fail(getUserResult.Error);

				var user = getUserResult.Data;

				if (!user.IsAdmin)
					return Result<T>.Fail(Errors.AccessDenied);

				// Выполняем действие
				return action();
			});
		}

		/// <summary>
		/// Выполнить операцию с обработкой исключений
		/// </summary>
		private Result<T> DoWithExceptionHandling<T>(Func<Result<T>> action)
		{
			try
			{
				// Выполняем действие
				return action();
			}
			catch (AuthException ex)
			{
				return Result<T>.Fail(ex.ErrorCodeWithDescription);
			}
			catch (Exception ex)
			{
				return Result<T>.Fail("Необработанное исключение", ex);
			}
		}
	}
}
