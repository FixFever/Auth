using Auth.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;

namespace Auth.Controllers
{
	public class PublicController : Controller
    {
		/// <summary>
		/// Авторизация
		/// </summary>
		[HttpPost("Login")]
		[SwaggerOperation(Tags = new[] { Api.Public })]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Guid), "Авторизация по логину и паролю")]
		public IActionResult Login([FromBody] LoginPasswordRequestDto loginPassword)
		{
			return Ok("Hello world!");
		}
	}
}
