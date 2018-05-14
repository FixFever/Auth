using System;
using System.Runtime.CompilerServices;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;

namespace Auth.Core.Services
{
	/// <summary>
	/// Сервис для действий администраторов
	/// </summary>
	public class AdminService : BaseService, IAdminService
	{
		/// <inheritdoc />
		public AdminService(IAuthRepository authRepository, ITokenController tokenController, ILogRepository logRepository, IOptions<AuthOptions> authOptions) 
			: base(authRepository, tokenController, logRepository, authOptions)
		{
		}

		/// <summary>
		/// Добавить нового пользователя в хранилище
		/// </summary>
		public Result<UserDto> CreateUser(UserRequestDto user, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, null, user, () =>
			{
				if (user == null || string.IsNullOrEmpty(user.Login))
					return Result<UserDto>.Fail(Errors.RequestIsEmpty);

				return AuthRepository.CreateUser(user);
			});
		}

		/// <summary>
		/// Блокирует/разблокирует пользователя
		/// </summary>
		public Result<bool> SetUserIsActive(int userId, bool IsActive, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, IsActive, () =>
			{
				var result = AuthRepository.SetUserIsActive(userId, IsActive);

				// Если блокируем пользователя, то удаляем его токен
				if (IsActive == false && result.IsSuccess)
				{
					var userToken = TokenController.GetTokenForUser(userId);
					if (userToken.HasValue)
						TokenController.RemoveToken(userToken.Value);
				}

				return result;
			});
		}

		/// <summary>
		/// Назначает/сбрасывает права админа
		/// </summary>
		public Result<bool> SetUserIsAdmin(int userId, bool IsAdmin, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, IsAdmin, () => AuthRepository.SetUserIsAdmin(userId, IsAdmin));
		}

		/// <summary>
		/// Сменить пароль пользователя
		/// </summary>
		public Result<bool> SetUserPassword(int userId, string password, Guid token)
		{
			return DoWithCheckAdminPermissionAndLog(token, userId, null, () =>
			{
				var result = AuthRepository.SetUserPassword(userId, password);

				// При успешной смене пароля сбрасываем токен пользователя
				if (result.IsSuccess)
				{
					var userToken = TokenController.GetTokenForUser(userId);
					if (userToken.HasValue)
						TokenController.RemoveToken(userToken.Value);
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

				return AuthRepository.UpdateUser(userId, user);
			});
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
				if (adminToken == AuthOptions.RootToken)
					return Result<int>.Success(0);

				var adminId = TokenController.GetUserId(adminToken);

				var getUserResult = AuthRepository.GetUserById(adminId);

				if (!getUserResult.IsSuccess)
					return Result<int>.Fail(getUserResult.Error);

				var adminUser = getUserResult.Data;

				if (!adminUser.IsAdmin)
					return Result<int>.Fail(Errors.AccessDenied);
				
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
					LogRepository.LogAdminAction(adminUserId, userId, data, methodName);

				return actionResult;
			}, methodName);
		}
	}
}
