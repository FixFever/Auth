using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Auth.Attributes
{
	/// <summary>
	/// Получает Guid-параметр из заголовка или из url запроса
	/// </summary>
	public class GuidBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != typeof(Guid) &&
				bindingContext.ModelType != typeof(Guid?))
				throw new Exception("Атрибут только для Guid");

			if (!bindingContext.BindingSource.CanAcceptDataFrom(BindingSource.Header))
				return Task.CompletedTask;

			var headerName = bindingContext.ModelName;

			// Берем значение из заголовка запроса
			var stringValue = bindingContext.HttpContext.Request.Headers[headerName];

			if (Guid.TryParse(stringValue, out var guid))
			{
				bindingContext.Result = ModelBindingResult.Success(guid);
			}
			else
			{
				if (bindingContext.ModelType == typeof(Guid))
				{
					bindingContext.ModelState.TryAddModelError("Error", headerName + " is required");
					return Task.CompletedTask;
				}
				else
					bindingContext.Result = ModelBindingResult.Success(null);
			}

			return Task.CompletedTask;
		}
	}
}
