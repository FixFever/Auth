using Microsoft.AspNetCore.Mvc;

namespace Auth.Attributes
{
	public class TokenAttribute : FromHeaderAttribute
	{
		public TokenAttribute()
		{
			Name = "token";
		}
	}
}
