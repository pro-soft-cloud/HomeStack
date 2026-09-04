using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HomeStack.Core.Models;

namespace HomeStack.Database.EntityConfigs;

internal sealed class HostInstanceConfiguration : IEntityTypeConfiguration<HostInstance>
{
	public void Configure(EntityTypeBuilder<HostInstance> builder)
	{
		builder
			.ToTable("HostInstances")
			.HasKey(x => x.Id);

		builder.Property(x => x.Id).ValueGeneratedOnAdd();
		builder.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();
		builder.Property(x => x.IP).IsRequired();
		builder.Property(x => x.SystemId).IsRequired();
		builder.Property(x => x.ValidFrom).IsRequired();
		builder.Property(x => x.ValidTo).IsRequired();
		builder.Property(x => x.CreatedAt).IsRequired();
		builder.Property(x => x.CreatedBy).IsRequired();
		builder.Property(x => x.LastUpdatedAt).IsRequired(false);
		builder.Property(x => x.LastUpdatedBy).IsRequired(false);

		builder.HasIndex(i => i.SystemId).IsUnique();
		builder.HasIndex(i => i.DisplayName).IsUnique();
	}
}
