namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Configurations;

/// <inheritdoc />
internal sealed class EventHandlerRelationshipConfiguration : IEntityTypeConfiguration<EventHandlerRelationship>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EventHandlerRelationship> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(r => r.EventId)
            .IsRequired();

        builder.Property(r => r.HandlerId)
            .IsRequired();

        builder.Property(r => r.EventHandlerRelationshipType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.Source)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.DeletionReason)
            .HasConversion<int?>();

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Индексы для оптимизации запросов
        builder.HasIndex(r => r.EventId)
            .HasDatabaseName("IX_EventHandlerRelationships_EventId");

        builder.HasIndex(r => r.HandlerId)
            .HasDatabaseName("IX_EventHandlerRelationships_HandlerId");

        builder.HasIndex(r => r.EventHandlerRelationshipType)
            .HasDatabaseName("IX_EventHandlerRelationships_RelationshipType");

        builder.HasIndex(r => r.Source)
            .HasDatabaseName("IX_EventHandlerRelationships_Source");

        builder.HasIndex(r => r.IsDeleted)
            .HasDatabaseName("IX_EventHandlerRelationships_IsDeleted");

        builder.HasIndex(r => r.UpdatedAt)
            .HasDatabaseName("IX_EventHandlerRelationships_UpdatedAt");

        // Уникальный индекс для предотвращения дублирования связей
        builder.HasIndex(r => new { r.EventId, r.HandlerId, r.Source })
            .IsUnique()
            .HasDatabaseName("IX_EventHandlerRelationships_EventId_HandlerId_Source");

        // Настройка внешних ключей и навигационных свойств
        builder.HasOne(r => r.Event)
            .WithMany(e => e.HandlerRelationships)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Handler)
            .WithMany(e => e.EventRelationships)
            .HasForeignKey(r => r.HandlerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Глобальный фильтр для мягкого удаления
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
