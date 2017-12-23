namespace P01_BillsPaymentSystem.Data.EntityConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(e => e.BankAccountId);

            builder.Property(e => e.Balance)
                .IsRequired();

            builder.Property(e => e.BankName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.SwiftCode)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(20);

            builder.Ignore(e => e.PaymentMethodId);
        }
    }
}
