using Microsoft.AspNetCore.Mvc;

namespace Auth.Attributes
{
	public class PasswordAttribute : FromHeaderAttribute
	{
		public PasswordAttribute()
		{
			Name = "password";
		}
	}
}
