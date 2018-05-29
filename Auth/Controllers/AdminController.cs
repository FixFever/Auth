using System;
using System.Net;
using Auth.Attributes;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Auth.Controllers
{
	[Route("api/user")]
	public class AdminController : BaseController
	{
		private readonly IAdminService _adminService;

		public AdminController(IAdminService adminService)
		{
			_adminService = adminService;
		}

		/// <summary>
		/// Создаёт пользователя
		/// </summary>
		[HttpPost()]
		[SwaggerOperation(Tags = new[] { Api.Admin })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>), "Создать пользователя")]
		public IActionResult CreateUser([FromBody] UserRequestDto user, [Token, GuidBinder] Guid token)
		{
			var result = _adminService.CreateUser(user, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Изменяет значения полей учетной записи пользователя
		/// </summary>
		[HttpPut("{userId:int}")]
		[SwaggerOperation(Tags = new[] { Api.Admin })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>), "Изменяет значения полей учетной записи пользователя")]
		public IActionResult UpdateUser(int userId, [FromBody] UserRequestDto user, [Token, GuidBinder] Guid token)
		{
			var result = _adminService.UpdateUser(userId, user, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Установить пароль для пользователя
		/// </summary>
		[HttpPut("{userId:int}/password")]
		[SwaggerOperation(Tags = new[] { Api.Admin })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<bool>), "Установить пароль для пользователя")]
		public IActionResult SetUserPassword([FromBody] PasswordRequestDto request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _adminService.SetUserPassword(userId, request.Password, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Заблокировать/разблокировать пользователя
		/// </summary>
		[HttpPut("{userId:int}/IsActive")]
		[SwaggerOperation(Tags = new[] { Api.Admin })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>), "Заблокировать/разблокировать пользователя")]
		public IActionResult SetIsActive([FromBody] BooleanRequest request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _adminService.SetUserIsActive(userId, request.Value, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Назначить/сбросить права админа
		/// </summary>
		[HttpPut("{userId:int}/IsAdmin")]
		[SwaggerOperation(Tags = new[] { Api.Admin })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>), "Назначить/сбросить права админа")]
		public IActionResult SetIsAdmin([FromBody] BooleanRequest request, int userId, [Token, GuidBinder] Guid token)
		{
			var result = _adminService.SetUserIsAdmin(userId, request.Value, token);
			return ConvertToActionResult(result);
		}
	}
}
