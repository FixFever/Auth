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
	public class PublicController : BaseController
	{
		private readonly IAuthService _authService;

		public PublicController(IAuthService authService)
		{
			_authService = authService;
		}
		
		/// <summary>
		/// Авторизация
		/// </summary>
		[HttpPost("Login")]
		[SwaggerOperation(Tags = new[] { Api.Public })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<Guid>), "Авторизация по логину и паролю, возвращает токен авторизации")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, typeof(Result<Guid>), "Ошибка авторизации")]
		public IActionResult Login([Login] string login, [Password] string password)
		{
			var result = _authService.Login(login, password);

			return ConvertToActionResult(result);
		}
	}
}
