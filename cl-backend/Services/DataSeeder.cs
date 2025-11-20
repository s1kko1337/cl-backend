using cl_backend.DbContexts;
using cl_backend.Models.Categories;
using cl_backend.Models.Products;
using cl_backend.Models.Sales;
using cl_backend.Models.User;
using cl_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.Services;

public class DataSeeder
{
    private readonly ApplicationContext _context;
    private readonly Random _random = new();

    public DataSeeder(ApplicationContext context)
    {
        _context = context;
    }

    public async Task SeedAllDataAsync()
    {
        // Проверяем, нужно ли заполнять данные
        if (await _context.Categories.AnyAsync())
        {
            Console.WriteLine("База данных уже содержит данные. Пропускаем заполнение.");
            return;
        }

        Console.WriteLine("Начинаем заполнение базы данных моковыми данными...");

        await SeedUsersAsync();
        await SeedCategoriesAsync();
        await SeedProductsAsync();
        await SeedProductImagesAsync();
        await SeedReviewsAsync();
        await SeedOrdersAsync();

        Console.WriteLine("Заполнение базы данных завершено успешно!");
    }

    private async Task SeedUsersAsync()
    {
        Console.WriteLine("Создание пользователей...");

        var users = new List<User>
        {
            new User
            {
                Login = "ivanov@mail.ru",
                Username = "Иванов Иван",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            },
            new User
            {
                Login = "petrova@yandex.ru",
                Username = "Петрова Мария",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            },
            new User
            {
                Login = "sidorov@gmail.com",
                Username = "Сидоров Алексей",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            },
            new User
            {
                Login = "kuznetsova@mail.ru",
                Username = "Кузнецова Анна",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            },
            new User
            {
                Login = "smirnov@yandex.ru",
                Username = "Смирнов Дмитрий",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            },
            new User
            {
                Login = "volkova@gmail.com",
                Username = "Волкова Елена",
                Password = AuthUtils.HashPassword("password123"),
                Role = "user"
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {users.Count} пользователей.");
    }

    private async Task SeedCategoriesAsync()
    {
        Console.WriteLine("Создание категорий...");

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Электроника",
                Description = "Смартфоны, ноутбуки, планшеты и другая электроника"
            },
            new Category
            {
                Name = "Одежда",
                Description = "Мужская и женская одежда на любой сезон"
            },
            new Category
            {
                Name = "Обувь",
                Description = "Спортивная, повседневная и официальная обувь"
            },
            new Category
            {
                Name = "Книги",
                Description = "Художественная литература, учебники, справочники"
            },
            new Category
            {
                Name = "Спорт и отдых",
                Description = "Спортивное оборудование и товары для активного отдыха"
            },
            new Category
            {
                Name = "Дом и сад",
                Description = "Товары для дома, сада и ремонта"
            },
            new Category
            {
                Name = "Красота и здоровье",
                Description = "Косметика, парфюмерия и товары для здоровья"
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {categories.Count} категорий.");
    }

    private async Task SeedProductsAsync()
    {
        Console.WriteLine("Создание товаров...");

        var categories = await _context.Categories.ToListAsync();
        var products = new List<Product>();

        // Электроника
        var electronics = categories.First(c => c.Name == "Электроника");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Смартфон Samsung Galaxy S23",
                Description = "Флагманский смартфон с камерой 200 МП, 8 ГБ ОЗУ, 256 ГБ встроенной памяти",
                Price = 79990,
                StockQuantity = 15,
                SKU = "ELECT-001",
                CategoryId = electronics.Id
            },
            new Product
            {
                Name = "Ноутбук ASUS VivoBook 15",
                Description = "15.6\" Full HD, Intel Core i5, 8 ГБ ОЗУ, SSD 512 ГБ",
                Price = 54990,
                StockQuantity = 8,
                SKU = "ELECT-002",
                CategoryId = electronics.Id
            },
            new Product
            {
                Name = "Наушники Sony WH-1000XM5",
                Description = "Беспроводные наушники с активным шумоподавлением",
                Price = 29990,
                StockQuantity = 25,
                SKU = "ELECT-003",
                CategoryId = electronics.Id
            },
            new Product
            {
                Name = "Умные часы Apple Watch Series 9",
                Description = "GPS, Always-On Retina дисплей, датчик здоровья",
                Price = 44990,
                StockQuantity = 12,
                SKU = "ELECT-004",
                CategoryId = electronics.Id
            }
        });

        // Одежда
        var clothing = categories.First(c => c.Name == "Одежда");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Мужская куртка Columbia",
                Description = "Зимняя куртка с утеплителем Omni-Heat, водонепроницаемая",
                Price = 12990,
                StockQuantity = 20,
                SKU = "CLOTH-001",
                CategoryId = clothing.Id
            },
            new Product
            {
                Name = "Женское пальто Zara",
                Description = "Классическое демисезонное пальто из шерсти",
                Price = 8990,
                StockQuantity = 15,
                SKU = "CLOTH-002",
                CategoryId = clothing.Id
            },
            new Product
            {
                Name = "Джинсы Levi's 501",
                Description = "Классические прямые джинсы, 100% хлопок",
                Price = 5990,
                StockQuantity = 30,
                SKU = "CLOTH-003",
                CategoryId = clothing.Id
            },
            new Product
            {
                Name = "Футболка Adidas Originals",
                Description = "Хлопковая футболка с логотипом, унисекс",
                Price = 2490,
                StockQuantity = 50,
                SKU = "CLOTH-004",
                CategoryId = clothing.Id
            }
        });

        // Обувь
        var shoes = categories.First(c => c.Name == "Обувь");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Кроссовки Nike Air Max 270",
                Description = "Спортивные кроссовки с амортизацией Air, дышащий верх",
                Price = 10990,
                StockQuantity = 18,
                SKU = "SHOES-001",
                CategoryId = shoes.Id
            },
            new Product
            {
                Name = "Ботинки Timberland Classic",
                Description = "Водонепроницаемые ботинки из натуральной кожи",
                Price = 14990,
                StockQuantity = 10,
                SKU = "SHOES-002",
                CategoryId = shoes.Id
            },
            new Product
            {
                Name = "Туфли мужские классические",
                Description = "Кожаные туфли для офиса и торжественных мероприятий",
                Price = 6990,
                StockQuantity = 15,
                SKU = "SHOES-003",
                CategoryId = shoes.Id
            }
        });

        // Книги
        var books = categories.First(c => c.Name == "Книги");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Мастер и Маргарита - Михаил Булгаков",
                Description = "Классический роман русской литературы, издание с иллюстрациями",
                Price = 890,
                StockQuantity = 40,
                SKU = "BOOK-001",
                CategoryId = books.Id
            },
            new Product
            {
                Name = "Война и мир - Лев Толстой (комплект из 2 книг)",
                Description = "Полное издание великого романа-эпопеи",
                Price = 1490,
                StockQuantity = 25,
                SKU = "BOOK-002",
                CategoryId = books.Id
            },
            new Product
            {
                Name = "Атомные привычки - Джеймс Клир",
                Description = "Как приобрести хорошие привычки и избавиться от плохих",
                Price = 790,
                StockQuantity = 35,
                SKU = "BOOK-003",
                CategoryId = books.Id
            }
        });

        // Спорт и отдых
        var sports = categories.First(c => c.Name == "Спорт и отдых");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Велосипед горный Stels Navigator 500",
                Description = "21 скорость, стальная рама, колеса 26 дюймов",
                Price = 24990,
                StockQuantity = 7,
                SKU = "SPORT-001",
                CategoryId = sports.Id
            },
            new Product
            {
                Name = "Гантели разборные 2х20 кг",
                Description = "Комплект разборных гантелей с хромированным грифом",
                Price = 4990,
                StockQuantity = 12,
                SKU = "SPORT-002",
                CategoryId = sports.Id
            },
            new Product
            {
                Name = "Коврик для йоги с разметкой",
                Description = "TPE материал, толщина 6 мм, нескользящая поверхность",
                Price = 1790,
                StockQuantity = 30,
                SKU = "SPORT-003",
                CategoryId = sports.Id
            }
        });

        // Дом и сад
        var home = categories.First(c => c.Name == "Дом и сад");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Пылесос робот Xiaomi Mi Robot",
                Description = "Умный робот-пылесос с построением карты помещения",
                Price = 19990,
                StockQuantity = 10,
                SKU = "HOME-001",
                CategoryId = home.Id
            },
            new Product
            {
                Name = "Комплект постельного белья Семейный",
                Description = "Сатин, 100% хлопок, европейский размер",
                Price = 3490,
                StockQuantity = 20,
                SKU = "HOME-002",
                CategoryId = home.Id
            },
            new Product
            {
                Name = "Набор кастрюль Tefal 5 предметов",
                Description = "Нержавеющая сталь, индукционное дно",
                Price = 8990,
                StockQuantity = 8,
                SKU = "HOME-003",
                CategoryId = home.Id
            }
        });

        // Красота и здоровье
        var beauty = categories.First(c => c.Name == "Красота и здоровье");
        products.AddRange(new[]
        {
            new Product
            {
                Name = "Электрическая зубная щетка Oral-B",
                Description = "Умная зубная щетка с датчиком давления и таймером",
                Price = 5490,
                StockQuantity = 15,
                SKU = "BEAUTY-001",
                CategoryId = beauty.Id
            },
            new Product
            {
                Name = "Фен для волос Dyson Supersonic",
                Description = "Профессиональный фен с технологией контроля температуры",
                Price = 34990,
                StockQuantity = 5,
                SKU = "BEAUTY-002",
                CategoryId = beauty.Id
            },
            new Product
            {
                Name = "Крем для лица Nivea",
                Description = "Увлажняющий крем для ежедневного ухода, 50 мл",
                Price = 490,
                StockQuantity = 50,
                SKU = "BEAUTY-003",
                CategoryId = beauty.Id
            }
        });

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {products.Count} товаров.");
    }

    private async Task SeedProductImagesAsync()
    {
        Console.WriteLine("Создание изображений товаров...");

        var products = await _context.Products.ToListAsync();
        var images = new List<ProductImage>();

        foreach (var product in products)
        {
            // Для каждого товара создаем 1-3 изображения
            int imageCount = _random.Next(1, 4);
            for (int i = 1; i <= imageCount; i++)
            {
                images.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = $"/uploads/products/{product.SKU.ToLower()}-{i}.jpg",
                    AltText = $"{product.Name} - изображение {i}"
                });
            }
        }

        await _context.ProductImages.AddRangeAsync(images);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {images.Count} изображений товаров.");
    }

    private async Task SeedReviewsAsync()
    {
        Console.WriteLine("Создание отзывов...");

        var products = await _context.Products.ToListAsync();
        var users = await _context.Users.Where(u => u.Role == "user").ToListAsync();
        var reviews = new List<ProductReview>();

        var reviewTemplates = new[]
        {
            new { Rating = 5, Comment = "Отличный товар! Полностью соответствует описанию. Рекомендую!" },
            new { Rating = 5, Comment = "Очень доволен покупкой. Качество на высоте, доставка быстрая." },
            new { Rating = 4, Comment = "Хороший товар, но цена могла быть и пониже. В целом доволен." },
            new { Rating = 4, Comment = "Качество хорошее, но упаковка была немного помята при доставке." },
            new { Rating = 5, Comment = "Превосходное качество! Уже не первый раз заказываю здесь." },
            new { Rating = 3, Comment = "Товар неплохой, но ожидал большего за такую цену." },
            new { Rating = 5, Comment = "Всё отлично! Быстрая доставка, товар в идеальном состоянии." },
            new { Rating = 4, Comment = "Соответствует описанию. Небольшие замечания по качеству сборки." },
            new { Rating = 5, Comment = "Рекомендую! Отличное соотношение цены и качества." },
            new { Rating = 5, Comment = "Замечательный товар! Пользуюсь уже месяц - никаких нареканий." },
            new { Rating = 4, Comment = "В целом доволен, хотя доставка заняла чуть больше времени." },
            new { Rating = 5, Comment = "Превзошло все ожидания! Спасибо продавцу!" }
        };

        var userNames = new[]
        {
            "Иван Иванов", "Мария Петрова", "Алексей Сидоров",
            "Екатерина Кузнецова", "Дмитрий Смирнов", "Анна Волкова",
            "Сергей Морозов", "Ольга Соколова", "Андрей Новиков"
        };

        // Для каждого товара создаем 2-5 отзывов
        foreach (var product in products.Take(20)) // Ограничиваем для первых 20 товаров
        {
            int reviewCount = _random.Next(2, 6);
            var shuffledUsers = users.OrderBy(x => _random.Next()).Take(reviewCount).ToList();

            for (int i = 0; i < reviewCount && i < shuffledUsers.Count; i++)
            {
                var template = reviewTemplates[_random.Next(reviewTemplates.Length)];
                var user = shuffledUsers[i];

                reviews.Add(new ProductReview
                {
                    ProductId = product.Id,
                    AuthorId = user.Id,
                    AuthorName = userNames[_random.Next(userNames.Length)],
                    Rating = template.Rating,
                    Comment = template.Comment,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 90)),
                    ReviewImageUrl = _random.Next(100) < 20 ? $"/uploads/reviews/review-{Guid.NewGuid()}.jpg" : null
                });
            }
        }

        await _context.ProductReviews.AddRangeAsync(reviews);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {reviews.Count} отзывов.");
    }

    private async Task SeedOrdersAsync()
    {
        Console.WriteLine("Создание заказов...");

        var products = await _context.Products.ToListAsync();
        var users = await _context.Users.Where(u => u.Role == "user").ToListAsync();
        var orders = new List<Order>();

        var customerData = new[]
        {
            new { Name = "Иван Петрович Иванов", Phone = "+79501234567", Address = "улица Ленина, 45, Москва, Московская область, Россия" },
            new { Name = "Мария Сергеевна Петрова", Phone = "+79507654321", Address = "проспект Победы, 12, Санкт-Петербург, Ленинградская область, Россия" },
            new { Name = "Алексей Владимирович Сидоров", Phone = "+79161234567", Address = "улица Шишкова, 30, Воронеж, Воронежская область, Россия" },
            new { Name = "Екатерина Николаевна Кузнецова", Phone = "+79267654321", Address = "улица Мира, 78, Краснодар, Краснодарский край, Россия" },
            new { Name = "Дмитрий Александрович Смирнов", Phone = "+79031234567", Address = "улица Гагарина, 15, Казань, Республика Татарстан, Россия" },
            new { Name = "Анна Викторовна Волкова", Phone = "+79197654321", Address = "проспект Мира, 54, Екатеринбург, Свердловская область, Россия" }
        };

        var statuses = new[] { "Pending", "Processing", "Completed", "Completed", "Completed" }; // Больше завершенных заказов
        var paymentMethods = new[] { "Card", "Card", "Cash" }; // Больше оплат картой

        // Создаем 15-20 заказов
        int orderCount = _random.Next(15, 21);

        for (int i = 0; i < orderCount; i++)
        {
            var user = users[_random.Next(users.Count)];
            var customer = customerData[_random.Next(customerData.Length)];
            var status = statuses[_random.Next(statuses.Length)];
            var paymentMethod = paymentMethods[_random.Next(paymentMethods.Length)];

            // Создаем заказ с 1-5 позициями
            var orderItems = new List<OrderItem>();
            int itemCount = _random.Next(1, 6);
            var selectedProducts = products.OrderBy(x => _random.Next()).Take(itemCount).ToList();

            decimal totalAmount = 0;

            foreach (var product in selectedProducts)
            {
                int quantity = _random.Next(1, 4);
                decimal subtotal = product.Price * quantity;
                totalAmount += subtotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PriceAtPurchase = product.Price,
                    Quantity = quantity,
                    Subtotal = subtotal
                });
            }

            var order = new Order
            {
                UserId = user.Id,
                CustomerName = customer.Name,
                CustomerPhone = customer.Phone,
                DeliveryAddress = customer.Address,
                PaymentMethod = paymentMethod,
                TotalAmount = totalAmount,
                Status = status,
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 60)),
                OrderItems = orderItems
            };

            if (status == "Completed")
            {
                order.UpdatedAt = order.CreatedAt.AddDays(_random.Next(1, 5));
            }

            orders.Add(order);
        }

        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Создано {orders.Count} заказов с {orders.Sum(o => o.OrderItems.Count)} позициями.");
    }
}
