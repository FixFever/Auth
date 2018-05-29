using Auth.Attributes;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;

namespace Auth.Controllers
{
	[Route("api")]
	public class AuthenticationController : BaseController
	{
		private readonly IAuthenticationService _authenticationService;

		public AuthenticationController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		/// <summary>
		/// Авторизация
		/// </summary>
		[HttpPost("Login")]
		[SwaggerOperation(Tags = new[] { Api.Public })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<Guid>), "Авторизация по логину и паролю, возвращает токен авторизации")]
		public IActionResult Login([Login] string login, [Password] string password)
		{
			var result = _authenticationService.Login(login, password);

			return ConvertToActionResult(result);
		}
	}
}
