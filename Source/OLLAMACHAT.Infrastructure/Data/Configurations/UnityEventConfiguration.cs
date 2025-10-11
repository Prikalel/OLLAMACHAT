namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class UnityEventConfiguration : IEntityTypeConfiguration<UnityEvent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UnityEvent> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.GenericTypeArguments)
            .HasConversion(
                v => v != null ? string.Join(",", v) : null,
                v => v != null ? v.Split(',', StringSplitOptions.RemoveEmptyEntries) : null)
            .Metadata.SetValueComparer(new ValueComparer<string[]>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c != null ? c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())) : 0,
                c => c != null ? c.ToArray() : Array.Empty<string>()));

        builder.Property(u => u.ArgumentsHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(u => u.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(u => u.BaseClassFullName)
            .HasMaxLength(500);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Индексы для оптимизации запросов
        builder.HasIndex(u => u.FullName)
            .IsUnique()
            .HasDatabaseName("IX_UnityEvents_FullName");

        builder.HasIndex(u => u.ArgumentsHash)
            .HasDatabaseName("IX_UnityEvents_ArgumentsHash");

        builder.HasIndex(u => u.FilePath)
            .HasDatabaseName("IX_UnityEvents_FilePath");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_UnityEvents_IsDeleted");

        builder.HasIndex(u => u.LastModified)
            .HasDatabaseName("IX_UnityEvents_LastModified");

        // Настройка навигационных свойств
        builder.HasMany(u => u.HandlerRelationships)
            .WithOne(r => r.Event)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
