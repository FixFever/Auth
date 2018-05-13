using Microsoft.AspNetCore.Mvc;

namespace Auth.Attributes
{
	public class LoginAttribute : FromHeaderAttribute
	{
		public LoginAttribute()
		{
			Name = "login";
		}
	}
}
