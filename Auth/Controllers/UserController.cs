using Auth.Attributes;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;

namespace Auth.Controllers
{
	[Route("api/user")]
	public class UserController : BaseController
	{
		private readonly IAuthService _authService;

		public UserController(IAuthService authService)
		{
			_authService = authService;
		}

		/// <summary>
		/// Создаёт пользователя
		/// </summary>
		[HttpPost()]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Создать пользователя")]
		public IActionResult CreateUser([FromBody] UserRequestDto user, [Token, GuidBinder] Guid token)
		{
			var result = _authService.CreateUser(user, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Изменяет значения полей учетной записи пользователя
		/// </summary>
		[HttpPut("{userId:int}")]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Изменяет значения полей учетной записи пользователя")]
		public IActionResult UpdateUser(int userId, [FromBody] UserRequestDto user, [Token, GuidBinder] Guid token)
		{
			var result = _authService.UpdateUser(userId, user, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Получить данные о пользователе по id
		/// </summary>
		[HttpGet("{userId:int}")]
		[SwaggerOperation(Tags = new[] { Api.Public })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>),
			"Получить данные о пользователе по id")]
		public IActionResult GetUser(int userId, [Token, GuidBinder] Guid token)
		{
			var result = _authService.GetUser(userId, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Получить данные обо всех пользователях
		/// </summary>
		[HttpGet()]
		[SwaggerOperation(Tags = new[] { Api.Public })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto[]>),
			"Получить данные обо всех пользователях")]
		public IActionResult GetAllUsers([Token, GuidBinder] Guid token)
		{
			var result = _authService.GetAllUsers(token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Установить пароль для пользователя
		/// </summary>
		[HttpPut("{userId:int}/password")]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Установить пароль для пользователя")]
		public IActionResult SetUserPassword([FromBody] PasswordRequestDto request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _authService.SetUserPassword(userId, request.Password, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Заблокировать/разблокировать пользователя
		/// </summary>
		[HttpPut("{userId:int}/IsActive")]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Заблокировать/разблокировать пользователя")]
		public IActionResult SetIsActive([FromBody] BooleanRequest request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _authService.SetUserIsActive(userId, request.Value, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Назначить/сбросить права админа
		/// </summary>
		[HttpPut("{userId:int}/IsAdmin")]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Назначить/сбросить права админа")]
		public IActionResult SetIsAdmin([FromBody] BooleanRequest request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _authService.SetUserIsAdmin(userId, request.Value, token);
			return ConvertToActionResult(result);
		}
	}
}
