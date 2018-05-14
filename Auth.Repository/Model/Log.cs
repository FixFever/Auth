using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Repository.Model
{
	public class Log
	{
		public int Id { get; set; }
		public DateTimeOffset DateTime { get; set; }
		public int? AdminUserId { get; set; }
		public int? UserId { get; set; }
		public string Action { get; set; }
		public string Text { get; set; }

		public class LogMap : IEntityTypeConfiguration<Log>
		{
			public void Configure(EntityTypeBuilder<Log> builder)
			{
				builder.ToTable("Log", "dbo");
				builder.HasKey(t => t.Id);
			}
		}
	}
}
