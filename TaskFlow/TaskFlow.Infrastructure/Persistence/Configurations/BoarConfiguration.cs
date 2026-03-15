using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public class BoarConfiguration :IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("boards");
        
        builder.HasKey(b=>b.Id);
        
        builder.Property(b=>b.Name).IsRequired().HasMaxLength(100);

        builder.Property(b => b.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasOne(b => b.Owner)
            .WithMany(u => u.OwnedBoards)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}