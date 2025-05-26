namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(uc => uc.Id)
            .IsRequired()
            // .UseIdentityAlwaysColumn() // Removed for SQLite compatibility
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Role)
            .IsRequired();
        builder.Property(x => x.Content)
            .IsRequired();
        builder.Property(x => x.Time)
            .IsRequired();
    }
}