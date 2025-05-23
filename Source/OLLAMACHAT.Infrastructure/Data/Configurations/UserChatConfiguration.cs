namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserChat> builder)
    {
        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(uc => uc.Name)
            .IsRequired();

        builder.Property(uc => uc.Model)
            .IsRequired();

        builder.Property(uc => uc.Active)
            .IsRequired();

        builder.Property(uc => uc.State)
            .IsRequired();

        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}