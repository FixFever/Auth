using System;
using System.Collections.Generic;

namespace Auth.Core
{
	public class AuthException : Exception
	{
		public KeyValuePair<string, string> ErrorCodeWithDescription;

		public AuthException(KeyValuePair<string, string> error)
			: base(error.Value)
		{
			ErrorCodeWithDescription = error;
		}
	}
}
