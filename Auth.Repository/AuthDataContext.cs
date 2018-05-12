using Auth.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repository
{
	public class AuthDataContext : DbContext
	{
		public AuthDataContext(DbContextOptions<AuthDataContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new User.UserMap());
			modelBuilder.ApplyConfiguration(new ChangeLog.ChangeLogMap());
		}
	}
}
