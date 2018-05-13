using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Repository.Model
{
	public class ChangeLog
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public DateTimeOffset DateTime { get; set; }
		public int AdminUserId { get; set; }
		public int UserId { get; set; }
		public string Text { get; set; }

		public class ChangeLogMap : IEntityTypeConfiguration<ChangeLog>
		{
			public void Configure(EntityTypeBuilder<ChangeLog> builder)
			{
				builder.ToTable("ChangeLog", "dbo");
				builder.HasKey(t => t.Id);
			}
		}
	}
}
