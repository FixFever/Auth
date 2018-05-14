using System;
using System.Linq;
using Auth.Core;
using Auth.Core.Interfaces;
using Auth.Core.Model;
using Auth.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repository
{
	public class AuthRepository : IAuthRepository
	{
		private readonly AuthDataContext _context;

		public AuthRepository(AuthDataContext context)
		{
			_context = context;
		}

		public Result<UserDto> Authenticate(string login, string password)
		{
			if (String.IsNullOrWhiteSpace(login))
				return Result<UserDto>.Fail(Errors.LoginIsEmpty);
			if (String.IsNullOrWhiteSpace(password))
				return Result<UserDto>.Fail(Errors.PasswordIsEmpty);

			var getUsetResult = WorkWithDb(db => GetUserByLoginPassword(login, password, db));

			if (!getUsetResult.IsSuccess)
				return Result<UserDto>.Fail(getUsetResult.Error);

			if (!getUsetResult.Data.IsActive)
				return Result<UserDto>.Fail(Errors.UserIsNonActive);

			return Result<UserDto>.Success(getUsetResult.Data.Dto);
		}

		public Result<UserDto> CreateUser(UserRequestDto user)
		{
			if (user == null) throw new ArgumentNullException("user");

			return WorkWithDb(db =>
			{
				if (db.Set<User>()
						.FirstOrDefault(u => u.Login.Equals(user.Login, StringComparison.InvariantCultureIgnoreCase)) != null)
					return Result<UserDto>.Fail(Errors.UserAlreadyExists);

				var newUser = MapNewUser(user);

				db.Set<User>().Add(newUser);
				db.SaveChanges();

				var createdUser = db.Set<User>()
					.Single(u => u.Login.Equals(user.Login, StringComparison.InvariantCultureIgnoreCase));

				return Result<UserDto>.Success(createdUser.Dto);
			});
		}

		public Result<UserDto[]> GetAllUsers()
		{
			return WorkWithDb(db =>
			{
				var users = db.Set<User>()
								.ToList();

				return Result<UserDto[]>.Success(users.Select(u => u.Dto).ToArray());
			});
		}

		public Result<bool> SetUserPassword(int userId, string password)
		{
			return WorkWithDb(db =>
			{
				var user = db.Set<User>().FirstOrDefault(u => u.Id == userId);
				if (user == null)
					return Result<bool>.Fail(Errors.UserNotFound);

				user.Password = PasswordUtils.CreatePasswordHash(password, user.Salt);
				db.SaveChanges();

				return Result<bool>.Success(true);
			});
		}

		public Result<bool> UpdateUser(int userId, UserRequestDto user)
		{
			return WorkWithDb(db =>
			{
				var record = db.Set<User>().FirstOrDefault(u => u.Id == userId);
				if (record == null)
					return Result<bool>.Fail(Errors.UserNotFound);

				if (!record.Login.Equals(user.Login, StringComparison.InvariantCultureIgnoreCase) &&
					db.Set<User>()
						.FirstOrDefault(u => u.Login.Equals(user.Login, StringComparison.InvariantCultureIgnoreCase)) != null)
					return Result<bool>.Fail(Errors.UserAlreadyExists);

				record.Login = user.Login;
				record.FullName = user.FullName;
				db.SaveChanges();

				return Result<bool>.Success(true);
			});
		}

		public Result<bool> SetUserIsAdmin(int userId, bool IsAdmin)
		{
			return WorkWithDb(db =>
			{
				var record = db.Set<User>().FirstOrDefault(u => u.Id == userId);
				if (record == null)
					return Result<bool>.Fail(Errors.UserNotFound);

				record.IsAdmin = IsAdmin;
				db.SaveChanges();

				return Result<bool>.Success(true);
			});
		}

		public Result<bool> SetUserIsActive(int userId, bool IsActive)
		{
			return WorkWithDb(db =>
			{
				var record = db.Set<User>().FirstOrDefault(u => u.Id == userId);
				if (record == null)
					return Result<bool>.Fail(Errors.UserNotFound);

				record.IsActive = IsActive;
				db.SaveChanges();

				return Result<bool>.Success(true);
			});
		}

		public Result<UserDto> GetUserById(int userId)
		{
			var result = WorkWithDb(db =>
			{
				var user = db
					.Set<User>()
					.FirstOrDefault(u => u.Id == userId);

				if (user == null)
					return Result<UserDto>.Fail(Errors.UserNotFound);

				return Result<UserDto>.Success(user.Dto);
			});

			return result;
		}

		private static Result<User> GetUserByLoginPassword(string login, string password, DbContext db)
		{
			var user = db.Set<User>()
				.FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));

			if (user == null)
				return Result<User>.Fail(Errors.WrongLoginOrPassword);

			if (user.Password != PasswordUtils.CreatePasswordHash(password, user.Salt))
				return Result<User>.Fail(Errors.WrongLoginOrPassword);

			return Result<User>.Success(user);
		}

		private static User MapNewUser(UserRequestDto user)
		{
			var password = PasswordUtils.GeneratePassword();
			var salt = Guid.NewGuid().ToString();
			return new User
			{
				Login = user.Login,
				FullName = user.FullName,
				IsActive = true,
				IsAdmin = false,
				Password = PasswordUtils.CreatePasswordHash(password, salt),
				Salt = salt,
			};
		}

		private T WorkWithDb<T>(Func<DbContext, T> action)
		{
			return action(_context);
		}
	}
}
