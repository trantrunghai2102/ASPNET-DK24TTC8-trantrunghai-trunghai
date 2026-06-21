using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Data;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        // Check if category "Nước giải khát" already exists
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == "Nước giải khát");
        if (category == null)
        {
            category = new Category
            {
                Name = "Nước giải khát"
            };
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded category 'Nước giải khát'.");
        }

        // Add products if they don't exist (based on Slug)
        var productsToSeed = new List<Product>
        {
            new()
            {
                CategoryId = category.CategoryId,
                Name = "Nước ngọt Coca Cola lon 320ml",
                Slug = "nuoc-ngot-coca-cola-lon-320ml",
                BasePrice = 10600,
                ShortDesc = "Nước ngọt có gas vị cola, lon 320ml.",
                DetailDesc = "Sản phẩm nước giải khát có gas, phù hợp dùng lạnh.",
                ThumbnailUrl = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_.jpg",
                StockQty = 100,
                Status = "active",
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = true },
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_8036d35a5d3d4e8db510845b2871101b_master.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = false },
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_925ef3e056f047c48c152399f8612801_master.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = false },
                }
            },
            new()
            {
                CategoryId = category.CategoryId,
                Name = "Nước ngọt Pepsi Cola lon 320ml",
                Slug = "nuoc-ngot-pepsi-cola-lon-320ml",
                BasePrice = 10600,
                ShortDesc = "Nước ngọt có gas vị cola, lon 320ml.",
                DetailDesc = "Pepsi Cola lon tiện lợi, dùng ngon hơn khi uống lạnh.",
                ThumbnailUrl = "/uploads/products/nuoc-ngot-pepsi-cola-lon-320ml-202403091730333958.jpg",
                StockQty = 100,
                Status = "active",
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/pepsi-338298393838.jpg", AltText = "Nước ngọt Pepsi Cola lon 320ml", IsPrimary = true },
                    new() { Url = "/uploads/products/depositphotos_677704846-stock-photo-odessa-ukraine-pepsi-cola-drink.jpg", AltText = "Nước ngọt Pepsi Cola lon 320ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = category.CategoryId,
                Name = "Nước tăng lực Sting hương dâu chai 330ml",
                Slug = "nuoc-tang-luc-sting-huong-dau-chai-330ml",
                BasePrice = 11500,
                ShortDesc = "Nước tăng lực hương dâu, chai 330ml.",
                DetailDesc = "Sting hương dâu giúp giải khát, bổ sung năng lượng.",
                ThumbnailUrl = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516182266.jpg",
                StockQty = 100,
                Status = "active",
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516182266.jpg", AltText = "Nước tăng lực Sting hương dâu chai 330ml", IsPrimary = true },
                    new() { Url = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516185862.jpg", AltText = "Nước tăng lực Sting hương dâu chai 330ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = category.CategoryId,
                Name = "Trà xanh Không Độ vị chanh chai 455ml",
                Slug = "tra-xanh-khong-do-vi-chanh-chai-455ml",
                BasePrice = 11300,
                ShortDesc = "Trà xanh vị chanh, chai 455ml.",
                DetailDesc = "Trà xanh Không Độ vị chanh thanh mát, phù hợp giải khát.",
                ThumbnailUrl = "/uploads/products/TXKD-PET-455ml-chiet-lanh-1.jpg",
                StockQty = 100,
                Status = "active",
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/TXKD-PET-455ml-chiet-lanh-1.jpg", AltText = "Trà xanh Không Độ vị chanh chai 455ml", IsPrimary = true },
                    new() { Url = "/uploads/products/tra_xanh_khong_do_loc_6_chai_x_500_ml_62b73195851c42559a438ea15fcbc612_master.jpg", AltText = "Trà xanh Không Độ vị chanh chai 455ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = category.CategoryId,
                Name = "Nước tăng lực Number1 chai 330ml",
                Slug = "nuoc-tang-luc-number1-chai-330ml",
                BasePrice = 11300,
                ShortDesc = "Nước tăng lực Number1 chai 330ml.",
                DetailDesc = "Nước tăng lực Number1 giúp giải khát và bổ sung năng lượng.",
                ThumbnailUrl = "/uploads/products/number1.jpg",
                StockQty = 100,
                Status = "active",
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/number1.jpg", AltText = "Nước tăng lực Number1 chai 330ml", IsPrimary = true },
                    new() { Url = "/uploads/products/nuoc-number-one.jpg", AltText = "Nước tăng lực Number1 chai 330ml", IsPrimary = false }
                }
            }
        };

        foreach (var p in productsToSeed)
        {
            var exists = await db.Products.AnyAsync(pr => pr.Slug == p.Slug);
            if (!exists)
            {
                db.Products.Add(p);
                logger.LogInformation("Queued seeding of product '{Name}'.", p.Name);
            }
        }

        await db.SaveChangesAsync();
    }
}
