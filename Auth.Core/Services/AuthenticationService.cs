using System;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.Extensions.Options;

namespace Auth.Core.Services
{
	public class AuthenticationService : BaseService, IAuthenticationService
	{
		/// <inheritdoc />
		public AuthenticationService(IAuthRepository authRepository, ITokenController tokenController, ILogRepository logRepository, IOptions<AuthOptions> authOptions)
			: base(authRepository, tokenController, logRepository, authOptions)
		{
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
				if (login == AuthOptions.RootLogin && password == AuthOptions.RootPassword)
					return Result<Guid?>.Success(AuthOptions.RootToken);

				var authenticateResult = AuthRepository.Authenticate(login, password);

				if (!authenticateResult.IsSuccess)
					return Result<Guid?>.Fail(authenticateResult.Error);

				var token = TokenController.CreateOrUpdateToken(authenticateResult.Data.Id);

				return Result<Guid?>.Success(token);
			});
		}
	}
}
