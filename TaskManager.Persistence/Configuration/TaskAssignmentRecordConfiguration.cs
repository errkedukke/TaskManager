using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain;

namespace TaskManager.Persistence.Configuration;

public sealed class TaskAssignmentRecordConfiguration : IEntityTypeConfiguration<TaskAssignmentRecord>
{
    public void Configure(EntityTypeBuilder<TaskAssignmentRecord> builder)
    {
        builder.ToTable("TaskAssignmentHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.AssignedAt)
            .IsRequired();

        builder.Property(x => x.TaskItemId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.TaskAssignmentRecords)
            .HasForeignKey(x => x.TaskItemId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => new { x.TaskItemId, x.UserId })
            .IsUnique();
    }
}
