using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public decimal Balance { get; set; }

        public string BankName { get; set; }

        public string SwiftCode { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount could not be negative");
            }

            if (amount > this.Balance)
            {
                throw new ArgumentException("Insufficient funds.");
            }
            this.Balance -= amount;
        }

        public void Deposit(decimal amount)
        {

            if (amount < 0)
            {
                throw new ArgumentException("Amount could not be negative");
            }
            this.Balance += amount;
        }

    }
}
