using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Auth.Repository.Model
{
	/// <summary>
	/// Данные пользователя для внутреннего использования сервисом
	/// </summary>
	public class User
    {
		public Guid Id { get; set; }

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
	}
}
