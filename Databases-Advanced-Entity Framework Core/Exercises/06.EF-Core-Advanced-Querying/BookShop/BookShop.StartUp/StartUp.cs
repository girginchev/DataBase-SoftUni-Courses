namespace BookShop
{
    using BookShop.Data;
    using BookShop.Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class StartUp
    {
        public static void Main()
        {
            //using (var db = new BookShopContext())
            //{
            //    DbInitializer.ResetDatabase(db);
            //}

            //var command = Console.ReadLine().ToLower().Trim();
            //using (var db = new BookShopContext())
            //{
            //    var result = GetBooksByAgeRestriction(db, command);
            //    Console.WriteLine(result);
            //}

            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetGoldenBooks(db));
            //}

            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBooksByPrice(db));
            //}

            //var year = int.Parse(Console.ReadLine());
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBooksNotRealeasedIn(db, year));
            //}

            //var input = Console.ReadLine();
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBooksByCategory(db, input));
            //}

            //var date = Console.ReadLine();
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBooksReleasedBefore(db,date));
            //}

            //var input = Console.ReadLine();
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetAuthorNamesEndingIn(db,input));
            //}

            //var input = Console.ReadLine().Trim();
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBookTitlesContaining(db,input));
            //}

            //var input = Console.ReadLine();
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetBooksByAuthor(db,input));
            //}

            //var lenghtCheck = int.Parse(Console.ReadLine());
            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(CountBooks(db,lenghtCheck));
            //}

            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(CountCopiesByAuthor(db));
            //}

            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetTotalProfitByCategory(db));
            //}

            //using (var db = new BookShopContext())
            //{
            //    Console.WriteLine(GetMostRecentBooks(db)); 
            //}

            //using (var db = new BookShopContext())
            //{
            //    IncreasePrices(db);
            //}

            using (var db = new BookShopContext())
            {
                int removedBooks = RemoveBooks(db);

                Console.WriteLine($"{removedBooks} books were deleted");
            }

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var books = new List<string>();
            switch (command)
            {
                case "adult":
                    books = context.Books.Where(e => e.AgeRestriction == Models.AgeRestriction.Adult).OrderBy(e => e.Title).Select(e => e.Title).ToList();
                    break;
                case "teen":
                    books = context.Books.Where(e => e.AgeRestriction == Models.AgeRestriction.Teen).OrderBy(e => e.Title).Select(e => e.Title).ToList();
                    break;
                case "minor":
                    books = context.Books.Where(e => e.AgeRestriction == Models.AgeRestriction.Minor).OrderBy(e => e.Title).Select(e => e.Title).ToList();
                    break;
            }

            var result = string.Join(Environment.NewLine, books);
            return result;

        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var result = context.Books
                .Where(e => e.EditionType == Models.EditionType.Gold && e.Copies < 5000)
                .OrderBy(e => e.BookId)
                .Select(e => e.Title)
                .ToList();
            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var result = context.Books
                .Where(e => e.Price > 40)
                .OrderByDescending(e => e.Price)
                .Select(e => new
                {
                    e.Title,
                    e.Price
                }).ToList();

            return string.Join(Environment.NewLine, result.Select(x => $"{x.Title} - ${x.Price:F2}"));
        }

        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var result = context.Books.Where(e => e.ReleaseDate.Value.Year != year).OrderBy(e => e.BookId).Select(e => e.Title).ToList();
            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(new char[] { '\t', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = context.Books.Where(e => e.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower()))).OrderBy(e=>e.Title)
                .Select(e=>e.Title).ToList();
            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.Where(e => e.ReleaseDate < releaseDate).OrderByDescending(e => e.ReleaseDate)
                .Select(e => new
                {
                    e.Title,
                    e.EditionType,
                    e.Price
                }).ToList();
            return string.Join(Environment.NewLine, books.Select(e => $"{e.Title} - {e.EditionType} - ${e.Price:F2}"));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors.Where(e => e.FirstName.EndsWith($"{input}", StringComparison.OrdinalIgnoreCase))
                .Select(e => $"{e.FirstName} {e.LastName}").OrderBy(e => e).ToList();

            return string.Join(Environment.NewLine, authors);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string pattern = $@"^.*{input}.*$";
            var titles = context.Books.Where(b => Regex.IsMatch(b.Title, pattern, RegexOptions.IgnoreCase)).Select(b => b.Title).OrderBy(x => x).ToArray();

            return string.Join(Environment.NewLine, titles);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var result = context.Books.Where(e => e.Author.LastName.StartsWith(input, StringComparison.OrdinalIgnoreCase)).OrderBy(e=>e.BookId)
                .Select(e => new
                {
                    e.Title,
                    Name = $"{e.Author.FirstName} {e.Author.LastName}"
                }).ToList();

            return string.Join(Environment.NewLine, result.Select(e => $"{e.Title} ({e.Name})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books.Where(e => e.Title.Length > lengthCheck).Count();
            return books;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var result = context.Books.Select(e => new
            {
                Name = $"{e.Author.FirstName} {e.Author.LastName}",
                Copies = e.Copies
            }).GroupBy(e=>e.Name).OrderByDescending(e=>e.Sum(x=>x.Copies)).ToList();
            return string.Join(Environment.NewLine, result.Select(e => $"{e.Key} - {e.Sum(x=>x.Copies)}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var result = context.Categories.Select(e => new
            {
                Category = e.Name,
                Profit = e.CategoryBooks.Sum(x => x.Book.Price * x.Book.Copies)
            }).GroupBy(e=>e.Category).OrderByDescending(e=>e.Sum(x=>x.Profit)).ThenBy(x=>x.Key).ToList();

            return  string.Join(Environment.NewLine, result.Select(e=> $"{e.Key} ${e.Sum(x=>x.Profit):F2}"));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var result = context.Categories.Select(e => new
            {
                Category = e.Name,
                CategoryCount = e.CategoryBooks.Count(),
                CategoryBooks = e.CategoryBooks
                .OrderByDescending(d=>d.Book.ReleaseDate.Value)
                .Take(3)
                .Select(x => new {
                    x.Book.Title,
                    x.Book.ReleaseDate.Value.Year
                }).OrderByDescending(x=>x.Year)
            }).OrderBy(e=>e.Category).ToList();

            return string.Join(Environment.NewLine, result.Select(x => $"--{x.Category}" + Environment.NewLine +
            string.Join(Environment.NewLine, x.CategoryBooks.Select(b => $"{b.Title} ({b.Year})"))));
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(e => e.ReleaseDate.Value.Year < 2010).ToList();

            foreach (var b in books)
            {
                b.Price += 5;
            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksForRemoval = context.Books.Where(e => e.Copies < 4200).ToList();
            foreach (var book in booksForRemoval)
            {
                context.Books.Remove(book);
            }

            context.SaveChanges();

            return booksForRemoval.Count();
        }
    }
}
