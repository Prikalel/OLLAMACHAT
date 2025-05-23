namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn()
            .IsRequired();

        builder.Property(u => u.Name)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique(true);

        builder.HasMany(u => u.Chats)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}