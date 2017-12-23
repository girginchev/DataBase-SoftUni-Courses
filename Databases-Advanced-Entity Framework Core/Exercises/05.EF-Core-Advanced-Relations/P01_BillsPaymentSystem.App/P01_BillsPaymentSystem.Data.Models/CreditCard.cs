using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }

        public decimal Limit { get; set; }

        public decimal MoneyOwed { get; set; }

        public DateTime ExpirationDate { get; set; }

        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount could not be negative");
            }

            if (amount > this.LimitLeft)
            {
                throw new ArgumentException("Insufficient funds.");
            }

            this.MoneyOwed += amount;
        }

        public void Deposit(decimal amount)
        {

            if (amount < 0)
            {
                throw new ArgumentException("Amount could not be negative");
            }
            this.MoneyOwed -= amount;
        }
    }
}
