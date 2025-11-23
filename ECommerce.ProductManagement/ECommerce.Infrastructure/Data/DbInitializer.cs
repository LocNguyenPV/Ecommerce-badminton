using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public static class DbInitializer
{
    private static int _idSeed = 1;

    public static async Task SeedAsync(ECommerceDbContext context)
    {
        // Clear existing data (only in development)
        if (!context.Products.Any())
        {
            await SeedCategoriesAsync(context);
            await SeedBrandsAsync(context);
            await SeedAttributesAsync(context);
            await SeedProductsAsync(context);

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedCategoriesAsync(ECommerceDbContext context)
    {
        var categories = new List<Category>
        {
            // Main Categories
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Badminton Rackets",
                Slug = "badminton-rackets",
                Description = "Professional and recreational badminton rackets",
                IsActive = true,
                SortOrder = 1
            },
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Badminton Shoes",
                Slug = "badminton-shoes",
                Description = "Court shoes designed for badminton",
                IsActive = true,
                SortOrder = 2
            },
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Badminton Apparel",
                Slug = "badminton-apparel",
                Description = "Clothing for badminton players",
                IsActive = true,
                SortOrder = 3
            },
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Shuttlecocks",
                Slug = "shuttlecocks",
                Description = "Feather and synthetic shuttlecocks",
                IsActive = true,
                SortOrder = 4
            },
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Bags & Accessories",
                Slug = "bags-accessories",
                Description = "Badminton bags, grips, and accessories",
                IsActive = true,
                SortOrder = 5
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBrandsAsync(ECommerceDbContext context)
    {
        var brands = new List<Brand>
        {
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Yonex",
                Description = "Leading badminton equipment manufacturer from Japan",
                LogoUrl = "https://example.com/logos/yonex.png",
                IsActive = true
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Victor",
                Description = "Premium badminton brand from Taiwan",
                LogoUrl = "https://example.com/logos/victor.png",
                IsActive = true
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Li-Ning",
                Description = "Chinese sports equipment brand",
                LogoUrl = "https://example.com/logos/lining.png",
                IsActive = true
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Apacs",
                Description = "Malaysian badminton equipment brand",
                LogoUrl = "https://example.com/logos/apacs.png",
                IsActive = true
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "FZ Forza",
                Description = "Danish badminton brand",
                LogoUrl = "https://example.com/logos/fzforza.png",
                IsActive = true
            }
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAttributesAsync(ECommerceDbContext context)
    {
        var attributes = new List<ECommerce.Domain.Entities.CustomAttribute>
        {
            // Racket attributes
            new() { AttributeId = Guid.NewGuid(), Name = "Weight", Type = "Racket", IsRequired = true, SortOrder = 1 },
            new() { AttributeId = Guid.NewGuid(), Name = "Grip Size", Type = "Racket", IsRequired = true, SortOrder = 2 },
            new() { AttributeId = Guid.NewGuid(), Name = "Balance Point", Type = "Racket", IsRequired = false, SortOrder = 3 },
            new() { AttributeId = Guid.NewGuid(), Name = "Flexibility", Type = "Racket", IsRequired = false, SortOrder = 4 },
            new() { AttributeId = Guid.NewGuid(), Name = "String Tension", Type = "Racket", IsRequired = false, SortOrder = 5 },
            
            // Shoe attributes
            new() { AttributeId = Guid.NewGuid(), Name = "Size", Type = "Shoe", IsRequired = true, SortOrder = 1 },
            new() { AttributeId = Guid.NewGuid(), Name = "Color", Type = "Shoe", IsRequired = true, SortOrder = 2 },
            new() { AttributeId = Guid.NewGuid(), Name = "Width", Type = "Shoe", IsRequired = false, SortOrder = 3 },
            
            // Apparel attributes
            new() { AttributeId = Guid.NewGuid(), Name = "Size", Type = "Apparel", IsRequired = true, SortOrder = 1 },
            new() { AttributeId = Guid.NewGuid(), Name = "Color", Type = "Apparel", IsRequired = true, SortOrder = 2 },
            new() { AttributeId = Guid.NewGuid(), Name = "Gender", Type = "Apparel", IsRequired = true, SortOrder = 3 }
        };

        await context.CustomAttributes.AddRangeAsync(attributes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(ECommerceDbContext context)
    {
        var racketCategory = await context.Categories.FirstAsync(c => c.Slug == "badminton-rackets");
        var shoeCategory = await context.Categories.FirstAsync(c => c.Slug == "badminton-shoes");
        var apparelCategory = await context.Categories.FirstAsync(c => c.Slug == "badminton-apparel");
        var shuttleCategory = await context.Categories.FirstAsync(c => c.Slug == "shuttlecocks");
        var accessoryCategory = await context.Categories.FirstAsync(c => c.Slug == "bags-accessories");

        var brands = await context.Brands.ToListAsync();
        var yonex = brands.First(b => b.Name == "Yonex");
        var victor = brands.First(b => b.Name == "Victor");
        var liNing = brands.First(b => b.Name == "Li-Ning");
        var apacs = brands.First(b => b.Name == "Apacs");
        var fzForza = brands.First(b => b.Name == "FZ Forza");

        var attributes = await context.CustomAttributes.ToListAsync();

        // Seed Badminton Rackets
        await SeedRacketsAsync(context, racketCategory, brands, attributes);

        // Seed Badminton Shoes
        await SeedShoesAsync(context, shoeCategory, brands, attributes);

        // Seed Apparel
        await SeedApparelAsync(context, apparelCategory, brands, attributes);

        // Seed Shuttlecocks
        await SeedShuttlecocksAsync(context, shuttleCategory, brands);

        // Seed Accessories
        await SeedAccessoriesAsync(context, accessoryCategory, brands);
    }

    private static async Task SeedRacketsAsync(
        ECommerceDbContext context,
        Category category,
        List<Brand> brands,
        List<ECommerce.Domain.Entities.CustomAttribute> attributes)
    {
        var racketModels = new[]
        {
            ("Astrox 99 Pro", "Yonex", "Head-heavy power racket for aggressive players", 189.99m, 95.00m),
            ("Arcsaber 11 Pro", "Yonex", "Even-balanced control racket", 179.99m, 90.00m),
            ("Nanoflare 1000Z", "Yonex", "Head-light speed racket", 169.99m, 85.00m),
            ("Thruster K 9900", "Victor", "Powerful offensive racket", 159.99m, 80.00m),
            ("Jetspeed S 12", "Victor", "Lightning-fast racket", 149.99m, 75.00m),
            ("Aeronaut 9000", "Li-Ning", "Professional tournament racket", 199.99m, 100.00m),
            ("Turbo Charging N9 II", "Li-Ning", "Explosive power racket", 139.99m, 70.00m),
            ("Nano Fusion 722 Speed", "Apacs", "Budget-friendly speed racket", 79.99m, 40.00m),
            ("Virtuoso Pro", "Apacs", "All-around performance racket", 89.99m, 45.00m),
            ("Power 988 M", "FZ Forza", "Maximum power racket", 119.99m, 60.00m),
            ("Light 4.1", "FZ Forza", "Ultra-lightweight racket", 109.99m, 55.00m),
            ("Duora 10 LT", "Yonex", "Dual-frame technology racket", 174.99m, 87.00m),
            ("TK Onigiri", "Victor", "Control-oriented racket", 144.99m, 72.00m),
            ("Windstorm 78", "Li-Ning", "Intermediate level racket", 99.99m, 50.00m)
        };

        var weightAttr = attributes.First(a => a.Name == "Weight" && a.Type == "Racket");
        var gripAttr = attributes.First(a => a.Name == "Grip Size" && a.Type == "Racket");
        var flexAttr = attributes.First(a => a.Name == "Flexibility" && a.Type == "Racket");

        var products = new List<Product>();

        foreach (var (model, brandName, desc, price, cost) in racketModels)
        {
            var brand = brands.First(b => b.Name == brandName);
            var sku = $"RACKET-{brandName.ToUpper().Replace(" ", "")}-{_idSeed:D4}";

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = sku,
                Name = $"{brandName} {model}",
                Description = desc,
                BasePrice = price,
                CostPrice = cost,
                CategoryId = category.CategoryId,
                BrandId = brand.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks,
                CreatedBy = "Seeder"
            };

            // Add variants (different weights and grip sizes)
            var weights = new[] { "3U (85-89g)", "4U (80-84g)", "5U (75-79g)" };
            var grips = new[] { "G4", "G5" };

            foreach (var weight in weights)
            {
                foreach (var grip in grips)
                {
                    var variant = new ProductVariant
                    {
                        VariantId = Guid.NewGuid(),
                        SKU = $"{sku}-{weight.Split(' ')[0]}-{grip}",
                        VariantName = $"{weight} / {grip}",
                        PriceAdjustment = 0m,
                        StockQuantity = new Random().Next(5, 50),
                        ReservedQuantity = new Random().Next(0, 5),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.Ticks,
                        UpdatedAt = DateTime.UtcNow.Ticks
                    };

                    variant.Attributes = new List<VariantAttribute>
                    {
                        new() { VariantAttributeId = Guid.NewGuid(), AttributeId = weightAttr.AttributeId, AttributeValue = weight },
                        new() { VariantAttributeId = Guid.NewGuid(), AttributeId = gripAttr.AttributeId, AttributeValue = grip },
                        new() { VariantAttributeId = Guid.NewGuid(), AttributeId = flexAttr.AttributeId, AttributeValue = new[] { "Stiff", "Medium", "Flexible" }[new Random().Next(3)] }
                    };

                    product.Variants.Add(variant);
                }
            }

            // Add product images
            product.Images = new List<ProductImage>
            {
                new()
                {
                    ImageId = Guid.NewGuid(),
                    Url = $"https://example.com/products/{sku}-front.jpg",
                    AltText = $"{product.Name} - Front View",
                    IsPrimary = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.UtcNow.Ticks
                },
                new()
                {
                    ImageId = Guid.NewGuid(),
                    Url = $"https://example.com/products/{sku}-side.jpg",
                    AltText = $"{product.Name} - Side View",
                    IsPrimary = false,
                    SortOrder = 2,
                    CreatedAt = DateTime.UtcNow.Ticks
                }
            };

            products.Add(product);
            _idSeed++;
        }

        await context.Products.AddRangeAsync(products);
    }

    private static async Task SeedShoesAsync(
        ECommerceDbContext context,
        Category category,
        List<Brand> brands,
        List<ECommerce.Domain.Entities.CustomAttribute> attributes)
    {
        var shoeModels = new[]
        {
            ("Power Cushion 65Z3", "Yonex", "Professional court shoes with cushioning", 129.99m, 65.00m),
            ("Aerus 3", "Yonex", "Lightweight tournament shoes", 139.99m, 70.00m),
            ("SH-A922", "Victor", "All-around court shoes", 99.99m, 50.00m),
            ("P9200TD", "Victor", "Durable training shoes", 89.99m, 45.00m),
            ("Saga 3", "Li-Ning", "Premium badminton shoes", 119.99m, 60.00m),
            ("Ranger TD", "Li-Ning", "Budget-friendly court shoes", 69.99m, 35.00m),
            ("Court Pro V2", "FZ Forza", "All-court performance shoes", 79.99m, 40.00m)
        };

        var sizeAttr = attributes.First(a => a.Name == "Size" && a.Type == "Shoe");
        var colorAttr = attributes.First(a => a.Name == "Color" && a.Type == "Shoe");

        var products = new List<Product>();
        var sizes = new[] { "US 7", "US 8", "US 9", "US 10", "US 11", "US 12" };
        var colors = new[] { "Black/Red", "White/Blue", "Navy/Yellow", "Black/White" };

        foreach (var (model, brandName, desc, price, cost) in shoeModels)
        {
            var brand = brands.First(b => b.Name == brandName);
            var sku = $"SHOE-{brandName.ToUpper().Replace(" ", "")}-{_idSeed:D4}";

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = sku,
                Name = $"{brandName} {model}",
                Description = desc,
                BasePrice = price,
                CostPrice = cost,
                CategoryId = category.CategoryId,
                BrandId = brand.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks,
                CreatedBy = "Seeder"
            };

            // Add variants (size and color combinations)
            foreach (var color in colors.Take(2)) // 2 colors per shoe
            {
                foreach (var size in sizes)
                {
                    var variant = new ProductVariant
                    {
                        VariantId = Guid.NewGuid(),
                        SKU = $"{sku}-{size.Replace(" ", "")}-{color.Split('/')[0]}",
                        VariantName = $"{size} - {color}",
                        PriceAdjustment = 0m,
                        StockQuantity = new Random().Next(10, 40),
                        ReservedQuantity = new Random().Next(0, 5),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.Ticks,
                        UpdatedAt = DateTime.UtcNow.Ticks
                    };

                    variant.Attributes = new List<VariantAttribute>
                    {
                        new() { VariantAttributeId = Guid.NewGuid(), AttributeId = sizeAttr.AttributeId, AttributeValue = size },
                        new() { VariantAttributeId = Guid.NewGuid(), AttributeId = colorAttr.AttributeId, AttributeValue = color }
                    };

                    product.Variants.Add(variant);
                }
            }

            product.Images = new List<ProductImage>
            {
                new()
                {
                    ImageId = Guid.NewGuid(),
                    Url = $"https://example.com/products/{sku}-main.jpg",
                    AltText = $"{product.Name}",
                    IsPrimary = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.UtcNow.Ticks
                }
            };

            products.Add(product);
            _idSeed++;
        }

        await context.Products.AddRangeAsync(products);
    }

    private static async Task SeedApparelAsync(
        ECommerceDbContext context,
        Category category,
        List<Brand> brands,
        List<ECommerce.Domain.Entities.CustomAttribute> attributes)
    {
        var apparelItems = new[]
        {
            ("Tournament Jersey", "Yonex", "Professional tournament jersey", 49.99m, 25.00m),
            ("Quick-Dry Shorts", "Victor", "Breathable badminton shorts", 34.99m, 17.00m),
            ("Court T-Shirt", "Li-Ning", "Moisture-wicking t-shirt", 29.99m, 15.00m),
            ("Training Polo", "FZ Forza", "Comfortable training polo", 39.99m, 20.00m)
        };

        var sizeAttr = attributes.First(a => a.Name == "Size" && a.Type == "Apparel");
        var colorAttr = attributes.First(a => a.Name == "Color" && a.Type == "Apparel");
        var genderAttr = attributes.First(a => a.Name == "Gender" && a.Type == "Apparel");

        var products = new List<Product>();
        var sizes = new[] { "S", "M", "L", "XL", "XXL" };
        var colors = new[] { "Navy", "Black", "Red", "Blue", "White" };
        var genders = new[] { "Men", "Women" };

        foreach (var (itemName, brandName, desc, price, cost) in apparelItems)
        {
            var brand = brands.First(b => b.Name == brandName);
            var sku = $"APPAREL-{brandName.ToUpper().Replace(" ", "")}-{_idSeed:D4}";

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = sku,
                Name = $"{brandName} {itemName}",
                Description = desc,
                BasePrice = price,
                CostPrice = cost,
                CategoryId = category.CategoryId,
                BrandId = brand.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks,
                CreatedBy = "Seeder"
            };

            foreach (var gender in genders)
            {
                foreach (var color in colors.Take(2))
                {
                    foreach (var size in sizes)
                    {
                        var variant = new ProductVariant
                        {
                            VariantId = Guid.NewGuid(),
                            SKU = $"{sku}-{gender[0]}-{size}-{color}",
                            VariantName = $"{gender} {size} - {color}",
                            PriceAdjustment = 0m,
                            StockQuantity = new Random().Next(20, 60),
                            ReservedQuantity = new Random().Next(0, 5),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow.Ticks,
                            UpdatedAt = DateTime.UtcNow.Ticks
                        };

                        variant.Attributes = new List<VariantAttribute>
                        {
                            new() { VariantAttributeId = Guid.NewGuid(), AttributeId = sizeAttr.AttributeId, AttributeValue = size },
                            new() { VariantAttributeId = Guid.NewGuid(), AttributeId = colorAttr.AttributeId, AttributeValue = color },
                            new() { VariantAttributeId = Guid.NewGuid(), AttributeId = genderAttr.AttributeId, AttributeValue = gender }
                        };

                        product.Variants.Add(variant);
                    }
                }
            }

            product.Images = new List<ProductImage>
            {
                new()
                {
                    ImageId = Guid.NewGuid(),
                    Url = $"https://example.com/products/{sku}.jpg",
                    AltText = $"{product.Name}",
                    IsPrimary = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.UtcNow.Ticks
                }
            };

            products.Add(product);
            _idSeed++;
        }

        await context.Products.AddRangeAsync(products);
    }

    private static async Task SeedShuttlecocksAsync(
        ECommerceDbContext context,
        Category category,
        List<Brand> brands)
    {
        var shuttles = new[]
        {
            ("Aerosensa 50", "Yonex", "Premium feather shuttlecocks", 29.99m, 15.00m),
            ("Mavis 350", "Yonex", "Durable nylon shuttlecocks", 19.99m, 10.00m),
            ("Champion No. 1", "Victor", "Tournament grade shuttlecocks", 27.99m, 14.00m),
            ("Gold Medal", "Victor", "Practice shuttlecocks", 22.99m, 12.00m),
            ("A+90", "Li-Ning", "Professional feather shuttles", 25.99m, 13.00m)
        };

        var products = new List<Product>();

        foreach (var (name, brandName, desc, price, cost) in shuttles)
        {
            var brand = brands.First(b => b.Name == brandName);
            var sku = $"SHUTTLE-{brandName.ToUpper().Replace(" ", "")}-{_idSeed:D4}";

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = sku,
                Name = $"{brandName} {name}",
                Description = $"{desc} - Pack of 12",
                BasePrice = price,
                CostPrice = cost,
                CategoryId = category.CategoryId,
                BrandId = brand.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks,
                CreatedBy = "Seeder"
            };

            // Single variant (no size/color variations for shuttles)
            var variant = new ProductVariant
            {
                VariantId = Guid.NewGuid(),
                SKU = $"{sku}-STD",
                VariantName = "Standard Pack",
                PriceAdjustment = 0m,
                StockQuantity = new Random().Next(50, 200),
                ReservedQuantity = new Random().Next(0, 10),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks
            };

            product.Variants.Add(variant);
            products.Add(product);
            _idSeed++;
        }

        await context.Products.AddRangeAsync(products);
    }

    private static async Task SeedAccessoriesAsync(
        ECommerceDbContext context,
        Category category,
        List<Brand> brands)
    {
        var accessories = new[]
        {
            ("Super Grap Overgrip", "Yonex", "Premium overgrip tape - 3 pack", 8.99m, 4.50m),
            ("Racket Bag Pro", "Victor", "Thermal badminton bag - Holds 6 rackets", 59.99m, 30.00m),
            ("Tournament Backpack", "Li-Ning", "Professional badminton backpack", 79.99m, 40.00m),
            ("Wrist Band Set", "FZ Forza", "Moisture-absorbing wrist bands", 9.99m, 5.00m),
            ("Grip Powder", "Victor", "Anti-slip grip powder", 6.99m, 3.50m)
        };

        var products = new List<Product>();

        foreach (var (name, brandName, desc, price, cost) in accessories)
        {
            var brand = brands.First(b => b.Name == brandName);
            var sku = $"ACC-{brandName.ToUpper().Replace(" ", "")}-{_idSeed:D4}";

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = sku,
                Name = $"{brandName} {name}",
                Description = desc,
                BasePrice = price,
                CostPrice = cost,
                CategoryId = category.CategoryId,
                BrandId = brand.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks,
                CreatedBy = "Seeder"
            };

            var variant = new ProductVariant
            {
                VariantId = Guid.NewGuid(),
                SKU = $"{sku}-STD",
                VariantName = "Standard",
                PriceAdjustment = 0m,
                StockQuantity = new Random().Next(30, 100),
                ReservedQuantity = new Random().Next(0, 5),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Ticks,
                UpdatedAt = DateTime.UtcNow.Ticks
            };

            product.Variants.Add(variant);
            products.Add(product);
            _idSeed++;
        }

        await context.Products.AddRangeAsync(products);
    }
}
