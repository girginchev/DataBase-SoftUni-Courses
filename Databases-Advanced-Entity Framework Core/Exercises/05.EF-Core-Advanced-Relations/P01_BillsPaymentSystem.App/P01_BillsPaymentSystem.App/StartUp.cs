namespace P01_BillsPaymentSystem.App
{
    using System;
    using P01_BillsPaymentSystem.Data;
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data.Models;
    using System.Globalization;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new BillsPaymentSystemContext())
            {
                db.Database.EnsureDeleted();
                db.Database.Migrate();
                Seed(db);

                //Get User Datails
                Console.Write("Enter user Id to get User Details: ");
                var idDetails = int.Parse(Console.ReadLine());
                GetUserDetails(db, idDetails);

                //Pay Bills
                Console.Write("Enter User Id for bills payment: ");
                var userId = int.Parse(Console.ReadLine());
                Console.Write("Enter amount for bills: ");
                var amount = decimal.Parse(Console.ReadLine());

                PayBills(userId, amount, db);

                GetUserDetails(db, userId);
            }

        }

        private static void GetUserDetails(BillsPaymentSystemContext db, int userId)
        {
            try
            {
                var user = db.Users.Where(e => e.UserId == userId)
                    .Select(e => new
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        CreditCards = e.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.CreditCard)
                        .Select(pm => new
                        {
                            CreditCard = pm.CreditCard
                        })
                        .ToList(),
                        BankAccounts = e.PaymentMethods.Where(pm => pm.Type == PaymentMethodType.BanckAccount)
                        .Select(pm => new
                        {
                            BankAccount = pm.BankAccount
                        })
                        .ToList()
                    }).FirstOrDefault();

                if (user == null)
                {
                    throw new ArgumentException("User Id not found.");
                }

                Console.WriteLine($"User: {user.FirstName} {user.LastName}");

                var bankAccounts = user.BankAccounts;
                if (bankAccounts.Any())
                {
                    Console.WriteLine("Bank Accounts:");
                }
                foreach (var ba in bankAccounts)
                {
                    Console.WriteLine($"-- ID: {ba.BankAccount.BankAccountId}");
                    Console.WriteLine($"--- Balance: {ba.BankAccount.Balance:F2}");
                    Console.WriteLine($"--- Bank: {ba.BankAccount.BankName}");
                    Console.WriteLine($"--- SWIFT: {ba.BankAccount.SwiftCode}");
                }

                var creditCards = user.CreditCards;
                if (creditCards.Any())
                {
                    Console.WriteLine("Credit Cards");
                }
                foreach (var cc in creditCards)
                {
                    Console.WriteLine($"-- ID: {cc.CreditCard.CreditCardId}");
                    Console.WriteLine($"--- Limit: {cc.CreditCard.Limit:F2}");
                    Console.WriteLine($"--- Money Owed: {cc.CreditCard.MoneyOwed:F2}");
                    Console.WriteLine($"--- Limit Left: {cc.CreditCard.LimitLeft:F2}");
                    Console.WriteLine($"--- Expiration Date: {cc.CreditCard.ExpirationDate.ToString("yyyy/MM", CultureInfo.InvariantCulture)}");
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public static void Seed(BillsPaymentSystemContext db)
        {
            var user = new User()
            {
                FirstName = "Guy",
                LastName = "Gilbert",
                Email = "gg@gmail.com",
                Password = "gugi",
            };

            var bankAccounts = new BankAccount[]
            {
                    new BankAccount()
                    {
                        Balance = 2000,
                        BankName = "Unicredit Bulbank",
                        SwiftCode = "UNCRBGSF",
                    },
                    new BankAccount()
                    {
                        Balance = 10000,
                        BankName = "Raiffaisen Bank",
                        SwiftCode = "RBBBG"
                    }
            };

            var creditCard = new CreditCard()
            {
                ExpirationDate = DateTime.ParseExact("20.12.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Limit = 5000,
                MoneyOwed = 2500,
            };

            var paymentMethods = new PaymentMethod[]
            {
                new PaymentMethod()
                {
                    User = user,
                    BankAccount = bankAccounts[0],
                    Type = PaymentMethodType.BanckAccount,
                },
                new PaymentMethod()
                {
                    User = user,
                    BankAccount = bankAccounts[1],
                    Type = PaymentMethodType.BanckAccount
                },
                new PaymentMethod()
                {
                    User = user,
                    CreditCard = creditCard,
                    Type = PaymentMethodType.CreditCard
                }
            };

            db.Users.Add(user);
            db.CreditCards.Add(creditCard);
            db.BankAccounts.AddRange(bankAccounts);
            db.PaymentMethods.AddRange(paymentMethods);

            db.SaveChanges();
        }

        public static void PayBills(int userId, decimal amount, BillsPaymentSystemContext db)
        {
            try
            {
                var user = db.Users.Include(e => e.PaymentMethods).Where(u => u.UserId == userId).FirstOrDefault();

                if (user == null)
                {
                    throw new ArgumentException("User Id does not exists.");
                }
                var userBankAccounts = db.BankAccounts.Where(e => e.PaymentMethod.UserId == userId).ToList();
                var userCreditCards = db.CreditCards.Include(b => b.PaymentMethod).Where(e => e.PaymentMethod.UserId == userId).ToList();


                var moneyAvailable = userBankAccounts.Sum(e => e.PaymentMethod.BankAccount.Balance) + userCreditCards.Sum(e => e.PaymentMethod.CreditCard.LimitLeft);

                if (amount > moneyAvailable)
                {
                    throw new ArgumentException("Insufficient funds");
                }
                else
                {
                    bool areBillsPaid = false;
                    foreach (var ba in userBankAccounts.OrderBy(e => e.PaymentMethod.BankAccount.BankAccountId))
                    {
                        if (amount <= ba.PaymentMethod.BankAccount.Balance)
                        {
                            ba.PaymentMethod.BankAccount.Withdraw(amount);
                            areBillsPaid = true;
                        }
                        else
                        {
                            amount -= ba.PaymentMethod.BankAccount.Balance;
                            ba.PaymentMethod.BankAccount.Withdraw(ba.PaymentMethod.BankAccount.Balance);
                        }
                        if (areBillsPaid)
                        {
                            db.SaveChanges();
                            return;
                        }
                    }
                    foreach (var cc in userCreditCards.OrderBy(c => c.PaymentMethod.CreditCard.CreditCardId))
                    {

                        if (amount <= cc.PaymentMethod.CreditCard.LimitLeft)
                        {
                            cc.PaymentMethod.CreditCard.Withdraw(amount);
                            areBillsPaid = true;
                        }
                        else
                        {
                            amount -= cc.PaymentMethod.CreditCard.LimitLeft;
                            cc.PaymentMethod.CreditCard.Withdraw(cc.PaymentMethod.CreditCard.LimitLeft);
                        }

                        if (areBillsPaid)
                        {
                            db.SaveChanges();
                            return;
                        }
                    }
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}

