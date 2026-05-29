using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProSoft.HomeStack.Core.Models;

namespace ProSoft.HomeStack.Database.EntityConfigs;

internal sealed class HomeStackInstanceConfiguration : IEntityTypeConfiguration<HomeStackInstance>
{
	public void Configure(EntityTypeBuilder<HomeStackInstance> builder)
	{
		builder.HasKey(x => x.Id);

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
	}
}
