using System;
using System.Collections.Generic;
using Auth.Core.Interfaces;
using Auth.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Auth.Repository
{
	public class LogRepository : ILogRepository
	{
		private readonly AuthDataContext _context;

		public LogRepository(AuthDataContext context)
		{
			_context = context;
		}

		/// <inheritdoc />
		public void LogAdminAction(int adminUserId, int? userId, object data, string methodName)
		{
			WorkWithDb(db =>
			{
				db.Set<Log>()
					.Add(new Log
					{
						AdminUserId = adminUserId,
						DateTime = DateTimeOffset.Now,
						UserId = userId,
						Action = methodName,
						Text = data == null ? null : $"Request: {JsonConvert.SerializeObject(data)}"
					});

				db.SaveChanges();
			});
		}

		/// <inheritdoc />
		public void LogError(int? userId, KeyValuePair<string, string> error, object data, string methodName)
		{
			WorkWithDb(db =>
			{
				db.Set<Log>()
					.Add(new Log
					{
						DateTime = DateTimeOffset.Now,
						UserId = userId,
						Action = methodName,
						Text = $"{error.Key}: {error.Value}." + (data == null ? null : $" Request: {JsonConvert.SerializeObject(data)}")
					});

				db.SaveChanges();
			});
		}

		/// <inheritdoc />
		public void LogError(int? userId, string message, object data, string methodName)
		{
			WorkWithDb(db =>
			{
				db.Set<Log>()
					.Add(new Log
					{
						DateTime = DateTimeOffset.Now,
						UserId = userId,
						Action = methodName,
						Text = message + (data == null ? null : $" Request: {JsonConvert.SerializeObject(data)}")
					});

				db.SaveChanges();
			});
		}

		/// <inheritdoc />
		public void LogException(int? userId, Exception exception, object data, string methodName)
		{
			WorkWithDb(db =>
			{
				db.Set<Log>()
					.Add(new Log
					{
						DateTime = DateTimeOffset.Now,
						UserId = userId,
						Action = methodName,
						Text = (data == null ? null : $"Request: {JsonConvert.SerializeObject(data)} ") +
							   $"{exception.GetType()}: {exception.Message}: {exception.StackTrace}"
					});

				db.SaveChanges();
			});
		}

		private void WorkWithDb(Action<DbContext> action)
		{
			try
			{
				action(_context);
			}
			catch (Exception e)
			{
				// ignored
			}
		}
	}
}
