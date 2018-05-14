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
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Получить данные о пользователе по id
		/// </summary>
		[HttpGet("{userId:int}")]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto>),
			"Получить данные о пользователе по id")]
		public IActionResult GetUser(int userId, [Token, GuidBinder] Guid token)
		{
			var result = _userService.GetUser(userId, token);
			return ConvertToActionResult(result);
		}

		/// <summary>
		/// Получить данные обо всех пользователях
		/// </summary>
		[HttpGet()]
		[SwaggerOperation(Tags = new[] { Api.Internal })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Result<UserDto[]>),
			"Получить данные обо всех пользователях")]
		public IActionResult GetAllUsers([Token, GuidBinder] Guid token)
		{
			var result = _userService.GetAllUsers(token);
			return ConvertToActionResult(result);
		}
	}
}
