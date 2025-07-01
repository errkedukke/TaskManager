using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain;

namespace TaskManager.Persistence.Configuration;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(t => t.Title)
            .IsUnique();

        builder.Property(t => t.State)
            .IsRequired();

        builder.Property(t => t.AssignedUserId).IsRequired(false);
        builder.Property(t => t.PreviouslyAssignedUserId).IsRequired(false);

        builder.HasOne(t => t.AssignedUser)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.PreviouslyAssignedUser)
            .WithMany()
            .HasForeignKey(t => t.PreviouslyAssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.AssignmentHistory)
            .WithMany()
            .UsingEntity(j => j.ToTable("TaskItemAssignmentHistory"));
    }
}
