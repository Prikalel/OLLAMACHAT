namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class EventHandlerConfiguration : IEntityTypeConfiguration<EventHandler>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EventHandler> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ParameterTypes)
            .HasConversion(
                v => v != null ? string.Join(",", v) : string.Empty,
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        builder.Property(e => e.ArgumentsHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.BaseClassFullName)
            .HasMaxLength(500);

        builder.Property(e => e.IsPublic)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Индексы для оптимизации запросов
        builder.HasIndex(e => e.FullName)
            .IsUnique()
            .HasDatabaseName("IX_EventHandlers_FullName");

        builder.HasIndex(e => e.ArgumentsHash)
            .HasDatabaseName("IX_EventHandlers_ArgumentsHash");

        builder.HasIndex(e => e.FilePath)
            .HasDatabaseName("IX_EventHandlers_FilePath");

        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_EventHandlers_IsDeleted");

        builder.HasIndex(e => e.LastModified)
            .HasDatabaseName("IX_EventHandlers_LastModified");

        builder.HasIndex(e => e.IsPublic)
            .HasDatabaseName("IX_EventHandlers_IsPublic");

        // Настройка навигационных свойств
        builder.HasMany(e => e.EventRelationships)
            .WithOne(r => r.Handler)
            .HasForeignKey(r => r.HandlerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
