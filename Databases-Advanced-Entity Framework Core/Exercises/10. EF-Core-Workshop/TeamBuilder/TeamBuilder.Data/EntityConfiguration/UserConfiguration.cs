using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamBuilder.Data.EntityConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.UserId);

            builder.HasAlternateKey(x => x.Username);

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.FirstName)
                .HasMaxLength(25);

            builder.Property(x => x.LastName)
                .HasMaxLength(25);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasMany(x => x.ReceivedInvitations)
                .WithOne(x => x.InvitedUser)
                .HasForeignKey(x => x.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CreatedEvents)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CreatedTeams)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
