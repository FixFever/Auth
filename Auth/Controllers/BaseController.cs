using System.Net;
using Auth.Core;
using Auth.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
	public abstract class BaseController : Controller
	{
		public IActionResult ConvertToActionResult<T>(Result<T> result)
		{
			if (result.IsSuccess)
				return Ok(result);

			if (result.Error.Key == Errors.AccessDenied.Key ||
				result.Error.Key == Errors.WrongLoginOrPassword.Key ||
				result.Error.Key == Errors.TokenIsNonActual.Key)
				return StatusCode((int)HttpStatusCode.Unauthorized, result);
			
			if (result.Error.Key == Errors.ConnectToDbError.Key ||
				result.Error.Key == Errors.UnexpectedError.Key)
				return StatusCode((int)HttpStatusCode.InternalServerError, result);

			return StatusCode((int)HttpStatusCode.BadRequest, result);
		}
	}
}
