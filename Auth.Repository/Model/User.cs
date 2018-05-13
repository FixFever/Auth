using Auth.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Repository.Model
{
	/// <summary>
	/// Данные пользователя для внутреннего использования сервисом
	/// </summary>
	public class User
    {
		public int Id { get; set; }

		public string Login { get; set; }

		public string FullName { get; set; }

		public string Password { get; set; }

		public string Salt { get; set; }
		
		public bool IsActive { get; set; }

		public bool IsAdmin { get; set; }

		public class UserMap : IEntityTypeConfiguration<User>
		{
			public void Configure(EntityTypeBuilder<User> builder)
			{
				builder.ToTable("User", "dbo");
				builder.HasKey(t => t.Id);
			}
		}

		/// <summary>
		/// Dto возвращаемый сервисом
		/// </summary>
		public UserDto Dto => new UserDto
		{
			Id = Id,
			FullName = FullName,
			Login = Login,
			IsActive = IsActive,
			IsAdmin = IsAdmin
		};
	}
}
