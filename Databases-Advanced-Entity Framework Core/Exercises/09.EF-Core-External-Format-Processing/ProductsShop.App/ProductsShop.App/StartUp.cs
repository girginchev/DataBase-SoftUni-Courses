using ProductsShop.Data;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using ProductsShop.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductsShop.App
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new ProductsShopContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            Console.WriteLine("Choose Processing, for Json: 1, for Xml: 2......");
            var command = int.Parse(Console.ReadLine());

            if (command == 1)
            {
                Console.WriteLine(InsertUsers());
                Console.WriteLine(InsertCategories());
                Console.WriteLine(InsertProducts());
                SetProductCategories();
                Console.WriteLine(GetProductsInRange());
                Console.WriteLine(GetSuccessfullySoldProducts());
                Console.WriteLine(GetCategoriesByProductsCount());
                Console.WriteLine(GetUsersAndProducts());
            }
            else if (command == 2)
            {
                Console.WriteLine(ImportUsersFromXml());
                Console.WriteLine(ImportCategoriesFromXml());
                Console.WriteLine(ImportProductsFromXml());
                Console.WriteLine(GetProductsInRangeXml());
                Console.WriteLine(GetUsersSoldProductsXml());
                Console.WriteLine(GetCategoriesByProductsCountXml());
                Console.WriteLine(GetUsersAndProductsXml()); 
            }
            else
            {
                Console.WriteLine("Invalid command!");
            }



        }

        public static string GetUsersAndProductsXml()
        {
            using (var db = new ProductsShopContext())
            {
                var allProducts = db
                .Products
                .Include(p => p.Seller)
                .Where(p => p.BuyerId != null)
                .ToArray();

                var sellersCount = allProducts
                    .Select(p => p.Seller)
                    .Count();

                var result = new
                {
                    usersCount = sellersCount,
                    users = allProducts
                        .Select(p => new
                        {
                            firstName = p.Seller.FirstName,
                            lastName = p.Seller.LastName,
                            age = p.Seller.Age,
                            soldProducts = new
                            {
                                count = p.Seller.SoldProducts.Count(),
                                products = p.Seller
                                    .SoldProducts
                                    .Select(ps => new
                                    {
                                        name = ps.Name,
                                        price = ps.Price
                                    })
                                    .ToArray()
                            }
                        })
                        .ToArray()
                };

                var xmlDoc = new XDocument(new XElement("Users",
                                                new XAttribute("count", result.usersCount)));

                foreach (var seller in result.users)
                {
                    var userChild = new XElement("user");

                    if (seller.firstName != null)
                    {
                        userChild.Add(new XAttribute("first-name", seller.firstName));
                    }

                    userChild.Add(new XAttribute("last-name", seller.lastName));

                    if (seller.age != null)
                    {
                        userChild.Add(new XAttribute("age", seller.age));
                    }

                    var soldProductsChild = new XElement("sold-products",
                                                new XAttribute("count", seller.soldProducts.count));

                    foreach (var product in seller.soldProducts.products)
                    {
                        soldProductsChild.Add(new XElement("product",
                                                new XAttribute("name", product.name),
                                                new XAttribute("price", product.price)));
                    }

                    userChild.Add(soldProductsChild);

                    xmlDoc.Root.Add(userChild);
                }


                var path = "users-and-products.xml";
                xmlDoc.Save(path);
                return $"Result from GetUsersAndProducts has been exported in file: {path}";
            }
        }

        public static string GetCategoriesByProductsCountXml()
        {
            using (var db = new ProductsShopContext())
            {
                var result = db.Categories
                    .Select(e => new
                    {
                        CategoryName = e.Name,
                        ProductCount = e.CategoryProducts.Count(),
                        ProductAveragePrice = e.CategoryProducts.Average(x => x.Product.Price),
                        TotalRevenue = e.CategoryProducts.Sum(p => p.Product.Price)
                    }).OrderByDescending(e=>e.ProductCount).ToList();

                var xmlDoc = new XDocument(new XElement("categories"));

                var categories = new List<XElement>();

                foreach (var r in result)
                {
                    categories.Add(new XElement("category", new XAttribute("name", r.CategoryName), 
                        new XElement("product-count", r.ProductCount),
                        new XElement("average-price", r.ProductAveragePrice),
                        new XElement("total-revenue", r.TotalRevenue)));
                }
                xmlDoc.Root.Add(categories);
                var path = "categories-by-products.xml";
                xmlDoc.Save(path);
                return $"Result from GetCategoriesByProductsCount has been exported in file: {path}";
            }
        }

        public static string GetUsersSoldProductsXml()
        {
            using (var db = new ProductsShopContext())
            {
                var resProducts = db.Products
                    .Include(e => e.Seller)
                    .Where(e => e.BuyerId != null)
                    .ToList();

                var resSellers = resProducts.Select(e => new
                {
                    FirstName = e.Seller.FirstName,
                    LastName = e.Seller.LastName,
                    SoldProducts = e.Seller.SoldProducts.Select(p => new
                    {
                        ProductName = p.Name,
                        ProductPrice = p.Price
                    }).ToList()
                }).OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

                var xmlDoc = new XDocument(new XElement("users"));

                foreach (var seller in resSellers)
                {
                    var user = new XElement("user");

                    if (seller.FirstName != null)
                    {
                        user.Add(new XAttribute("first-name", seller.FirstName));
                    }

                    user.Add(new XAttribute("last-name", seller.LastName));

                    var soldProducts = new List<XElement>();

                    foreach (var product in seller.SoldProducts)
                    {
                        var childOfSoldProducts = new XElement("product",
                                            new XElement("name", product.ProductName),
                                            new XElement("price", product.ProductPrice));

                        soldProducts.Add(childOfSoldProducts);
                    }

                    user.Add(new XElement("sold-products", soldProducts));

                    xmlDoc.Root.Add(user);
                }

                var path = "user-sold-products.xml";
                xmlDoc.Save(path);
                return $"Result from UsersSoldProducts has been exported in file: {path}";
            }
        }

        public static string GetProductsInRangeXml()
        {
            using (var db = new ProductsShopContext())
            {
                var result = db.Products.Include(e => e.Buyer).Where(e => e.BuyerId != null && e.Price >= 1000 && e.Price <= 2000)
                    .Select(e => new
                    {
                        ProductName = e.Name,
                        ProductPrice = e.Price,
                        BuyerName = $"{e.Buyer.FirstName ?? ""} {e.Buyer.LastName}"
                    })
                    .OrderByDescending(e => e.ProductPrice)
                    .ToList();
                var xmlDoc = new XDocument(new XElement("products"));
                foreach (var p in result)
                {
                    xmlDoc.Root.Add(new XElement("product",
                        new XAttribute("name", p.ProductName),
                        new XAttribute("price", p.ProductPrice),
                        new XAttribute("buyer", p.BuyerName)));
                }
                var path = "products-in-range.xml";
                xmlDoc.Save(path);
                return $"Result from GetProductsInRange has been exported in file: {path}";
            }
        }

        public static string ImportProductsFromXml()
        {
            var path = "Files/products.xml";

            var xmlString = File.ReadAllText(path);

            var xmlDoc = XDocument.Parse(xmlString);

            var xmlElement = xmlDoc.Root.Elements();

            using (var db = new ProductsShopContext())
            {
                var userIds = db.Users.Select(e => e.Id).ToList();

                var categoryIds = db.Categories.Select(e => e.Id).ToList();

                var categoryProducts = new List<CategoryProduct>();

                foreach (var e in xmlElement)
                {
                    var name = e.Element("name").Value;
                    var price = decimal.Parse(e.Element("price").Value);

                    var random = new Random();
                    var sellerId = random.Next(1, userIds.Count + 1);

                    int? buyerId = random.Next(1, userIds.Count + 1);
                    if (buyerId == sellerId || buyerId - sellerId < 8)
                    {
                        buyerId = null;
                    }

                    var product = new Product
                    {
                        Name = name,
                        Price = price,
                        SellerId = sellerId,
                        BuyerId = buyerId,
                    };

                    var categoryId = random.Next(1, categoryIds.Count + 1);
                    var categoryProduct = new CategoryProduct
                    {
                        Product = product,
                        CategoryId = categoryId
                    };
                    categoryProducts.Add(categoryProduct);
                }
                db.CategoryProducts.AddRange(categoryProducts);
                db.SaveChanges();

                return $"{categoryProducts.Count} products has been inserted from file: {path}";

            }
        }

        public static string ImportCategoriesFromXml()
        {
            var path = "Files/categories.xml";

            var xmlString = File.ReadAllText(path);

            var xmlDoc = XDocument.Parse(xmlString);

            var xmlElement = xmlDoc.Root.Elements();

            var categories = new List<Category>();

            foreach (var e in xmlElement)
            {
                var catName = e.Element("name").Value;
                var category = new Category
                {
                    Name = catName
                };
                categories.Add(category);
            }

            using (var db = new ProductsShopContext())
            {
                db.Categories.AddRange(categories);
                db.SaveChanges();
            }

            return $"{categories.Count()} categories has been inserted from file: {path}";
        }

        public static string ImportUsersFromXml()
        {
            var path = "Files/users.xml";

            var xmlString = File.ReadAllText(path);

            var xmlDoc = XDocument.Parse(xmlString);

            var elements = xmlDoc.Root.Elements();

            var users = new List<User>();

            foreach (var e in elements)
            {
                string firstName = e.Attribute("firstName")?.Value;
                string lastName = e.Attribute("lastName").Value;

                int? age = null;
                if (e.Attribute("age") != null)
                {
                    age = int.Parse(e.Attribute("age").Value);
                }

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age
                };
                users.Add(user);
            }
            using (var db = new ProductsShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }

            return $"{users.Count} users has been imported from file: {path}";

        }


        public static string GetUsersAndProducts()
        {
            using (var db = new ProductsShopContext())
            {
                var products = db.Products.Include(e => e.Seller).Where(e => e.BuyerId != null).ToList();

                var userCount = products.Select(e => e.Seller).Count();

                var result = new
                {
                    usersCount = userCount,
                    users = products.Select(e => new
                    {
                        firstName = e.Seller.FirstName,
                        lastName = e.Seller.LastName,
                        age = e.Seller.Age,
                        soldProducts = new
                        {
                            count = e.Seller.SoldProducts.Count(),
                            products = e.Seller.SoldProducts.Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            }).ToList()
                        }
                    }).ToList()
                };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                var path = "users-and-products.json";
                File.WriteAllText(path, json);
                return $"Result from UsersAndProducts has been exported in {path}";
            }

        }

        public static string GetCategoriesByProductsCount()
        {
            using (var db = new ProductsShopContext())
            {
                var result = db.Categories.Include(e => e.CategoryProducts).OrderBy(e => e.Name).Select(e => new
                {
                    CategoryName = e.Name,
                    ProductsCount = e.CategoryProducts.Count(),
                    ProductsAveragePrice = e.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = e.CategoryProducts.Sum(p => p.Product.Price)
                }).ToList();
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                var path = "categories-by-products-count.json";
                File.WriteAllText(path, json);
                return $"Result from CategoriesByProductsCount has been exported in {path}";
            }
        }

        public static string GetSuccessfullySoldProducts()
        {
            using (var db = new ProductsShopContext())
            {
                var result = db.Users.Where(e => e.SoldProducts.All(s => s.BuyerId != null))
                    .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        SoldProducts = e.SoldProducts.Select(p => new
                        {
                            ProductName = p.Name,
                            ProductPrice = p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        }).ToList()
                    }).ToList();
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                var path = "users-sold-products.json";
                File.WriteAllText(path, json);
                return $"Result from SuccessfullySoldProducts has been exported in {path}";
            }
        }

        public static string GetProductsInRange()
        {
            using (var db = new ProductsShopContext())
            {
                var result = db.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(p => p.Price).Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        SellerName = $"{p.Seller.FirstName} {p.Seller.LastName}"
                    }).ToList();
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                var path = "products-in-range.json";
                File.WriteAllText(path, json);
                return $"Result from ProductsInRange has been exported in {path}";
            }
  
        }

        public static void SetProductCategories()
        {
            using (var db = new ProductsShopContext())
            {
                int[] productsIds = db.Products.AsNoTracking().Select(e => e.Id).ToArray();
                int[] categoriesIds = db.Categories.AsNoTracking().Select(e => e.Id).ToArray();

                var categoryproducts = new List<CategoryProduct>();

                foreach (var productId in productsIds)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var random = new Random();
                        var categoryId = random.Next(1, categoriesIds.Length + 1);
                        while (categoryproducts.Any(e => e.ProductId == productId && e.CategoryId == categoryId))
                        {
                            categoryId = random.Next(1, categoriesIds.Length + 1);
                        }
                        var categoryProduct = new CategoryProduct()
                        {
                            ProductId = productId,
                            CategoryId = categoryId
                        };
                        categoryproducts.Add(categoryProduct);
                    }
                }
                db.CategoryProducts.AddRange(categoryproducts);
                db.SaveChanges();
            }
        }

        public static string InsertProducts()
        {
            var productsPath = "Files/products.json";

            var products = ImprotJson<Product>(productsPath);
            var random = new Random();
            using (var db = new ProductsShopContext())
            {
                foreach (var product in products)
                {
                    int[] userIds = db.Users.Select(e => e.Id).ToArray();
                    var sellerId = random.Next(1, userIds.Length + 1);
                    int? buyerId = random.Next(1, userIds.Length + 1);
                    if (buyerId == sellerId || buyerId - sellerId < 10)
                    {
                        buyerId = null;
                    }

                    product.SellerId = sellerId;
                    product.BuyerId = buyerId;
                }

                db.Products.AddRange(products);
                db.SaveChanges();
            }
            var result = $"{products.Length} has been added from file: {productsPath}";
            return result;
        }

        public static string InsertCategories()
        {
            var categoryPath = "Files/categories.json";
            var categories = ImprotJson<Category>(categoryPath);

            using (var db = new ProductsShopContext())
            {
                db.Categories.AddRange(categories);
                db.SaveChanges();
            }

            var result = $"{categories.Length} has been imported from file: {categoryPath}";
            return result;
        }

        public static string InsertUsers()
        {
            var userPath = "Files/users.json";

            var users = ImprotJson<User>(userPath);

            using (var db = new ProductsShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }

            var result = $"{users.Length} has been imported from file: {userPath}";
            return result;
        }

        public static T[] ImprotJson<T>(string path)
        {
            var jsonString = File.ReadAllText(path);

            var objects = JsonConvert.DeserializeObject<T[]>(jsonString);

            return objects;
        }
    }
}

