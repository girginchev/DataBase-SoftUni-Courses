﻿namespace P01_BillsPaymentSystem.Data.EntityConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserId);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(80);

            builder.Property(e => e.Password)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(25);
        }
    }
}
