using Microsoft.AspNetCore.Mvc;

namespace Auth.Attributes
{
	public class GuidBinderAttribute : ModelBinderAttribute
	{
		public GuidBinderAttribute()
		{
			BinderType = typeof(GuidBinder);
		}
	}
}
