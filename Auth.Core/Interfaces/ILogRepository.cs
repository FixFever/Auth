using System;
using System.Collections.Generic;

namespace Auth.Core.Interfaces
{
	public interface ILogRepository
	{
		void LogAdminAction(int adminUserId, int? userId, object data, string methodName);

		void LogError(int? userId, KeyValuePair<string, string> error, object data, string methodName);

		void LogError(int? userId, string message, object data, string methodName);

		void LogException(int? userId, Exception exception, object data, string methodName);
	}
}