using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BanCaPheNuocGiaiKhat.Data;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        var categoryNames = new[] { "Cà phê hạt", "Cà phê bột", "Dụng cụ pha chế", "Nước giải khát" };
        var categories = new Dictionary<string, Category>();

        foreach (var name in categoryNames)
        {
            var cat = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (cat == null)
            {
                cat = new Category { Name = name };
                db.Categories.Add(cat);
                await db.SaveChangesAsync();
                logger.LogInformation("Seeded category '{Name}'.", name);
            }
            categories[name] = cat;
        }

        var now = DateTime.UtcNow;

        var productsToSeed = new List<Product>
        {
            // Cà phê hạt (CategoryId = 1)
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Ethiopia Yirgacheffe",
                Slug = "ethiopia-yirgacheffe",
                BasePrice = 220000m,
                PromotionPrice = null,
                ShortDesc = "Hương hoa cỏ với nốt hương nhài và cam chanh tươi sáng. Rang sáng màu.",
                DetailDesc = "Cà phê Ethiopia Yirgacheffe được chế biến khô (tự nhiên), mang lại hương vị sạch và tươi sáng. Nổi bật với hương hoa cỏ và cam chanh.",
                ThumbnailUrl = "/images/product-ethiopian.png",
                StockQty = 50,
                ViewCount = 12,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Colombia Supremo",
                Slug = "colombia-supremo",
                BasePrice = 190000m,
                PromotionPrice = null,
                ShortDesc = "Thể chất êm dịu, cân bằng với vị ngọt caramel và hậu vị hạt dẻ. Rang vừa.",
                DetailDesc = "Hạt cà phê Colombia Supremo được lựa chọn kỹ lưỡng và rang ở mức độ vừa để cân bằng độ chua tự nhiên với hương vị hạt dẻ.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 35,
                ViewCount = 8,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Sumatra Mandheling",
                Slug = "sumatra-mandheling",
                BasePrice = 210000m,
                PromotionPrice = null,
                ShortDesc = "Thể chất đậm đà, hương đất và phong phú với độ chua thấp. Rang đậm.",
                DetailDesc = "Cà phê Sumatra Mandheling nổi tiếng với thể chất đậm đà và độ chua thấp, đặc trưng bởi hương gỗ tuyết tùng và thảo mộc độc đáo.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 40,
                ViewCount = 20,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Artisan Espresso Blend",
                Slug = "artisan-espresso-blend",
                BasePrice = 240000m,
                PromotionPrice = null,
                ShortDesc = "Lớp crema dày mịn, hương sô-cô-la đen và hạnh nhân rang. Rang kiểu espresso.",
                DetailDesc = "Hỗn hợp espresso đặc trưng của chúng tôi kết hợp hạt cà phê từ Nam Mỹ và Châu Phi, được rang đậm để làm nổi bật hương vị ca cao đậm đà và hạt rang thơm.",
                ThumbnailUrl = "/images/product-artisan-espresso.png",
                StockQty = 25,
                ViewCount = 15,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Guatemala Antigua",
                Slug = "guatemala-antigua",
                BasePrice = 200000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị cay nồng và khói nhẹ với độ chua táo giòn. Rang vừa.",
                DetailDesc = "Hạt cà phê Guatemala Antigua được trồng trên đất núi lửa, mang lại hương vị khói độc đáo kết hợp với vị chua nhẹ của cam chanh.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 30,
                ViewCount = 5,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Kenya AA",
                Slug = "kenya-aa",
                BasePrice = 230000m,
                PromotionPrice = null,
                ShortDesc = "Độ chua sáng với hương quả lý chua đen và quả mọng.",
                DetailDesc = "Cà phê Kenya AA được chế biến bằng phương pháp ướt, mang lại hương vị cực kỳ đậm đà và sạch.",
                ThumbnailUrl = "/images/product-ethiopian.png",
                StockQty = 45,
                ViewCount = 28,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Brazil Santos",
                Slug = "brazil-santos",
                BasePrice = 180000m,
                PromotionPrice = null,
                ShortDesc = "Độ chua thấp, thể chất êm dịu với hương sô-cô-la ngọt ngào.",
                DetailDesc = "Hạt cà phê Brazil Santos cổ điển, được rang nhằm tôn vinh vị ngọt tự nhiên của hạt.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 60,
                ViewCount = 33,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Costa Rica Tarrazu",
                Slug = "costa-rica-tarrazu",
                BasePrice = 200000m,
                PromotionPrice = null,
                ShortDesc = "Độ chua thanh thoát với hương mật ong ngọt ngào và cam chanh.",
                DetailDesc = "Vùng Tarrazu cung cấp một trong những loại hạt cà phê ngon nhất ở Trung Mỹ.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 25,
                ViewCount = 19,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Java Preanger",
                Slug = "java-preanger",
                BasePrice = 220000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị truyền thống từ Indonesia với các nốt hương thảo mộc ngọt ngào.",
                DetailDesc = "Hạt cà phê Java Preanger từ vùng cao nguyên núi lửa phía Tây Java.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 30,
                ViewCount = 7,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Cà phê Vietnam Robusta Đặc biệt",
                Slug = "vietnam-robusta-special",
                BasePrice = 160000m,
                PromotionPrice = null,
                ShortDesc = "Hạt cà phê Robusta đậm vị, lượng caffeine cao từ Buôn Ma Thuột.",
                DetailDesc = "Hạt cà phê Robusta Buôn Ma Thuột chất lượng cao với hương vị đậm đà truyền thống.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 80,
                ViewCount = 42,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Peru Cajamarca",
                Slug = "peru-cajamarca",
                BasePrice = 190000m,
                PromotionPrice = null,
                ShortDesc = "Thể chất êm dịu với độ chua nhẹ của táo và nốt hương caramel.",
                DetailDesc = "Hạt cà phê Peru nguồn gốc đơn vùng (Single Origin) cao cấp.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 35,
                ViewCount = 13,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Jamaica Blue Mountain",
                Slug = "jamaica-blue-mountain",
                BasePrice = 650000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị nhẹ nhàng dịu êm, không đắng và có nốt hương thảo mộc ngọt ngào.",
                DetailDesc = "Cà phê Jamaica Blue Mountain đích thực với hậu vị mượt mà và sạch.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 10,
                ViewCount = 50,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Hawaii Kona Extra Fancy",
                Slug = "hawaii-kona-extra-fancy",
                BasePrice = 550000m,
                PromotionPrice = null,
                ShortDesc = "Đậm đà, êm dịu và hương vị sâu lắng với hương thơm của các loại hạt.",
                DetailDesc = "Được trồng trên các sườn núi Hualalai và Mauna Loa thuộc các vùng Bắc và Nam Kona.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 15,
                ViewCount = 45,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Tanzania Peaberry",
                Slug = "tanzania-peaberry",
                BasePrice = 240000m,
                PromotionPrice = null,
                ShortDesc = "Độ chua sáng với hương quả lý chua đen và sô-cô-la.",
                DetailDesc = "Hạt cà phê Peaberry được thu hoạch từ sườn núi Kilimanjaro.",
                ThumbnailUrl = "/images/product-ethiopian.png",
                StockQty = 25,
                ViewCount = 12,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Rwanda Bourbon",
                Slug = "rwanda-bourbon",
                BasePrice = 210000m,
                PromotionPrice = null,
                ShortDesc = "Vị ngọt kết hợp hương hoa cỏ và chút vị đường caramel.",
                DetailDesc = "Giống cà phê Bourbon được trồng ở các vùng có độ cao lớn của Rwanda.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 30,
                ViewCount = 20,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Burundi Ngozi",
                Slug = "burundi-ngozi",
                BasePrice = 220000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị trái cây phức hợp với hậu vị ngọt ngào và sạch.",
                DetailDesc = "Được thu hoạch từ tỉnh Ngozi, mang lại các nốt hương trái cây tươi sáng và mọng nước.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 20,
                ViewCount = 18,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Yemen Mocha Mattari",
                Slug = "yemen-mocha-mattari",
                BasePrice = 350000m,
                PromotionPrice = null,
                ShortDesc = "Hương đất, vị phức hợp với nốt hương sô-cô-la đậm và rượu vang.",
                DetailDesc = "Một trong những dòng cà phê lâu đời và truyền thống nhất, được trồng trên các ruộng bậc thang khô cằn ở Yemen.",
                ThumbnailUrl = "/images/product-artisan-espresso.png",
                StockQty = 12,
                ViewCount = 35,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Panama Geisha",
                Slug = "panama-geisha",
                BasePrice = 850000m,
                PromotionPrice = null,
                ShortDesc = "Cực kỳ tinh tế với hương hoa nhài thơm ngát và vị cam bergamot.",
                DetailDesc = "Giống cà phê cao cấp cực kỳ được săn đón, nổi tiếng với thể chất nhẹ như trà và hương hoa cỏ phong phú.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 5,
                ViewCount = 80,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê hạt"].CategoryId,
                Name = "Cà phê tách caffeine Decaf Swiss Water",
                Slug = "decaf-swiss-water",
                BasePrice = 200000m,
                PromotionPrice = null,
                ShortDesc = "Êm dịu và đậm đà, được tách caffeine tự nhiên không dùng hóa chất.",
                DetailDesc = "Được xử lý bằng phương pháp Swiss Water Method giúp giữ lại 99.9% hương vị nguyên bản mà không còn caffeine.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 40,
                ViewCount = 15,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },

            // Cà phê bột (CategoryId = 2)
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột Ethiopian Sidamo",
                Slug = "ethiopian-sidamo-ground",
                BasePrice = 210000m,
                PromotionPrice = null,
                ShortDesc = "Hương hoa cỏ với các nốt hương của chanh và đường mía.",
                DetailDesc = "Cà phê bột Ethiopian Sidamo đã được xay sẵn, thích hợp cho pha phin hoặc pha nhỏ giọt.",
                ThumbnailUrl = "/images/product-ethiopian.png",
                StockQty = 30,
                ViewCount = 14,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột French Roast Blend",
                Slug = "french-roast-blend",
                BasePrice = 190000m,
                PromotionPrice = null,
                ShortDesc = "Rang siêu đậm và hương khói với thể chất đậm đà.",
                DetailDesc = "Cà phê bột rang kiểu Pháp, lý tưởng để pha bằng bình French Press.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 40,
                ViewCount = 11,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột Italian Roast Espresso",
                Slug = "italian-roast-espresso-ground",
                BasePrice = 190000m,
                PromotionPrice = null,
                ShortDesc = "Rang đậm đà, tối ưu hóa cho pha chế espresso.",
                DetailDesc = "Cà phê xay mịn rang kiểu Ý, được tối ưu hóa cho các dòng máy pha Espresso.",
                ThumbnailUrl = "/images/product-artisan-espresso.png",
                StockQty = 50,
                ViewCount = 22,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột House Blend",
                Slug = "house-blend-ground",
                BasePrice = 170000m,
                PromotionPrice = null,
                ShortDesc = "Hỗn hợp cà phê cân bằng cho việc pha chế hàng ngày.",
                DetailDesc = "Cà phê bột phối trộn đặc trưng từ các hạt Nam Mỹ và Châu Phi.",
                ThumbnailUrl = "/images/product-artisan-espresso.png",
                StockQty = 70,
                ViewCount = 16,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột Breakfast Blend",
                Slug = "breakfast-blend-ground",
                BasePrice = 160000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị tươi sáng, thanh mát khởi đầu ngày mới.",
                DetailDesc = "Hỗn hợp cà phê được phối trộn hoàn hảo từ các hạt Nam Mỹ và Trung Mỹ.",
                ThumbnailUrl = "/images/product-colombia.png",
                StockQty = 60,
                ViewCount = 22,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột Vanilla Nut",
                Slug = "vanilla-nut-ground",
                BasePrice = 170000m,
                PromotionPrice = null,
                ShortDesc = "Hương thơm vani ngọt ngào và hạt dẻ nướng.",
                DetailDesc = "Cà phê bột cao cấp được ướp với hương vị vani và hạt phỉ tự nhiên.",
                ThumbnailUrl = "/images/product-artisan-espresso.png",
                StockQty = 45,
                ViewCount = 30,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột Hazelnut Cream",
                Slug = "hazelnut-cream-ground",
                BasePrice = 170000m,
                PromotionPrice = null,
                ShortDesc = "Hương vị hạt phỉ béo ngậy đậm đà.",
                DetailDesc = "Cà phê bột thơm mịn với hậu vị kem hạt phỉ đặc trưng.",
                ThumbnailUrl = "/images/product-guatemala.png",
                StockQty = 40,
                ViewCount = 28,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Cà phê bột"].CategoryId,
                Name = "Cà phê bột xay thô pha Cold Brew",
                Slug = "cold-brew-coarse-ground",
                BasePrice = 190000m,
                PromotionPrice = null,
                ShortDesc = "Được xay thô chuyên dụng cho phương pháp ngâm ủ nước lạnh.",
                DetailDesc = "Hỗn hợp cà phê rang đậm được xay thô hoàn hảo để tự làm cold brew mượt mà tại nhà.",
                ThumbnailUrl = "/images/hero-coffee-beans.png",
                StockQty = 55,
                ViewCount = 42,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },

            // Dụng cụ pha chế (CategoryId = 3)
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Phễu lọc sứ Hario V60",
                Slug = "hario-v60-ceramic",
                BasePrice = 250000m,
                PromotionPrice = null,
                ShortDesc = "Phễu lọc cà phê bằng sứ cổ điển cho phương pháp pha pour-over.",
                DetailDesc = "Phễu lọc cà phê pour-over bằng sứ cao cấp của Nhật Bản.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 20,
                ViewCount = 9,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Bình pha cà phê Chemex 6-Cup",
                Slug = "chemex-6-cup",
                BasePrice = 450000m,
                PromotionPrice = null,
                ShortDesc = "Bình pha cà phê bằng thủy tinh thanh lịch cho hương vị sạch và thuần khiết.",
                DetailDesc = "Bình pha cà phê thủy tinh Chemex đi kèm đai gỗ và dây da trang nhã.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 15,
                ViewCount = 18,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Bình pha cà phê Aeropress Go",
                Slug = "aeropress-go",
                BasePrice = 390000m,
                PromotionPrice = null,
                ShortDesc = "Dụng cụ ép cà phê nhỏ gọn, tiện lợi khi đi du lịch và dã ngoại.",
                DetailDesc = "Phiên bản du lịch Aeropress đi kèm cốc tiện lợi và hộp đựng di động.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 25,
                ViewCount = 25,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Ấm pha cà phê Moka Pot Bialetti",
                Slug = "moka-pot-bialetti",
                BasePrice = 350000m,
                PromotionPrice = null,
                ShortDesc = "Ấm pha cà phê espresso trên bếp mang tính biểu tượng của Ý.",
                DetailDesc = "Ấm pha espresso trên bếp Bialetti Moka Express dung tích 3 tách.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 18,
                ViewCount = 21,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Phễu lọc inox Kalita Wave",
                Slug = "kalita-wave-stainless",
                BasePrice = 300000m,
                PromotionPrice = null,
                ShortDesc = "Phễu lọc cà phê đáy phẳng giúp chiết xuất đồng đều.",
                DetailDesc = "Phễu lọc pour-over inox 185 Kalita Wave chất lượng cao.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 12,
                ViewCount = 10,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Máy xay cà phê tay Porlex Mini",
                Slug = "porlex-mini-grinder",
                BasePrice = 550000m,
                PromotionPrice = null,
                ShortDesc = "Máy xay cà phê cầm tay mini với lưỡi xay bằng gốm sứ.",
                DetailDesc = "Máy xay cà phê bằng tay Porlex Mini bằng thép không gỉ với lưỡi nghiền bằng gốm bền bỉ.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 10,
                ViewCount = 31,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Ấm pha cà phê cổ ngỗng",
                Slug = "gooseneck-kettle",
                BasePrice = 400000m,
                PromotionPrice = null,
                ShortDesc = "Ấm bằng thép không gỉ với vòi cổ ngỗng giúp kiểm soát dòng chảy chính xác.",
                DetailDesc = "Ấm đun nước cổ ngỗng dùng trên bếp để kiểm soát dòng chảy nước rót chính xác.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 15,
                ViewCount = 17,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Bình pha cà phê French Press (1L)",
                Slug = "french-press-maker-1l",
                BasePrice = 280000m,
                PromotionPrice = null,
                ShortDesc = "Bình pha French Press bằng thủy tinh cổ điển với bộ lọc thép không gỉ.",
                DetailDesc = "Bình pha cà phê French Press dung tích 1 Lít mang lại trải nghiệm cà phê trọn vị.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 25,
                ViewCount = 18,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Cân điện tử pha cà phê",
                Slug = "digital-coffee-scale",
                BasePrice = 220000m,
                PromotionPrice = null,
                ShortDesc = "Cân điện tử chính xác tích hợp chức năng bấm giờ.",
                DetailDesc = "Độ chính xác lên đến 0.1g, lý tưởng cho việc đong đo khi pha pour-over và căn chỉnh espresso.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 30,
                ViewCount = 34,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Hộp đập bã cà phê Knock Box",
                Slug = "coffee-knock-box",
                BasePrice = 180000m,
                PromotionPrice = null,
                ShortDesc = "Hộp đập bã bền bỉ chứa bánh bã cà phê espresso.",
                DetailDesc = "Hộp đập bã bằng nhựa chắc chắn với thanh gõ cao su có thể tháo rời để dễ dàng vệ sinh.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 40,
                ViewCount = 12,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Ca đánh sữa inox (350ml)",
                Slug = "milk-frothing-pitcher",
                BasePrice = 150000m,
                PromotionPrice = null,
                ShortDesc = "Ca inox (350ml) chuyên dụng tạo bọt sữa vẽ latte art.",
                DetailDesc = "Ca đánh sữa bằng thép không gỉ cổ điển với vòi rót chuẩn xác để tạo hình latte art.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 50,
                ViewCount = 21,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CategoryId = categories["Dụng cụ pha chế"].CategoryId,
                Name = "Tay nén cà phê Tamper (58mm)",
                Slug = "espresso-tamper-58mm",
                BasePrice = 250000m,
                PromotionPrice = null,
                ShortDesc = "Tamper bằng thép không gỉ nguyên khối với tay cầm công thái học.",
                DetailDesc = "Tay nén 58mm được thiết kế cân bằng hoàn hảo nhằm đảm bảo nén bánh cà phê espresso phẳng và đồng đều.",
                ThumbnailUrl = "/images/category-accessories.png",
                StockQty = 20,
                ViewCount = 26,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            },

            // Nước giải khát (CategoryId = 4)
            new()
            {
                CategoryId = categories["Nước giải khát"].CategoryId,
                Name = "Nước ngọt Coca Cola lon 320ml",
                Slug = "nuoc-ngot-coca-cola-lon-320ml",
                BasePrice = 10600m,
                PromotionPrice = null,
                ShortDesc = "Nước ngọt có gas vị cola, lon 320ml.",
                DetailDesc = "Sản phẩm nước giải khát có gas, phù hợp dùng lạnh.",
                ThumbnailUrl = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_.jpg",
                StockQty = 100,
                ViewCount = 0,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now,
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = true },
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_320_ml_8036d35a5d3d4e8db510845b2871101b_master.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = false },
                    new() { Url = "/uploads/products/nuoc_ngot_cocacola_vi_nguyen_ban_925ef3e056f047c48c152399f8612801_master.jpg", AltText = "Nước ngọt Coca Cola lon 320ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = categories["Nước giải khát"].CategoryId,
                Name = "Nước ngọt Pepsi Cola lon 320ml",
                Slug = "nuoc-ngot-pepsi-cola-lon-320ml",
                BasePrice = 10600m,
                PromotionPrice = null,
                ShortDesc = "Nước ngọt có gas vị cola, lon 320ml.",
                DetailDesc = "Pepsi Cola lon tiện lợi, dùng ngon hơn khi uống lạnh.",
                ThumbnailUrl = "/uploads/products/nuoc-ngot-pepsi-cola-lon-320ml-202403091730333958.jpg",
                StockQty = 100,
                ViewCount = 0,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now,
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/pepsi-338298393838.jpg", AltText = "Nước ngọt Pepsi Cola lon 320ml", IsPrimary = true },
                    new() { Url = "/uploads/products/depositphotos_677704846-stock-photo-odessa-ukraine-pepsi-cola-drink.jpg", AltText = "Nước ngọt Pepsi Cola lon 320ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = categories["Nước giải khát"].CategoryId,
                Name = "Nước tăng lực Sting hương dâu chai 330ml",
                Slug = "nuoc-tang-luc-sting-huong-dau-chai-330ml",
                BasePrice = 11500m,
                PromotionPrice = null,
                ShortDesc = "Nước tăng lực hương dâu, chai 330ml.",
                DetailDesc = "Sting hương dâu giúp giải khát, bổ sung năng lượng.",
                ThumbnailUrl = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516182266.jpg",
                StockQty = 100,
                ViewCount = 0,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now,
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516182266.jpg", AltText = "Nước tăng lực Sting hương dâu chai 330ml", IsPrimary = true },
                    new() { Url = "/uploads/products/nuoc-tang-luc-sting-dau-pet-330ml_202509291516185862.jpg", AltText = "Nước tăng lực Sting hương dâu chai 330ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = categories["Nước giải khát"].CategoryId,
                Name = "Trà xanh Không Độ vị chanh chai 455ml",
                Slug = "tra-xanh-khong-do-vi-chanh-chai-455ml",
                BasePrice = 11300m,
                PromotionPrice = null,
                ShortDesc = "Trà xanh vị chanh, chai 455ml.",
                DetailDesc = "Trà xanh Không Độ vị chanh thanh mát, phù hợp giải khát.",
                ThumbnailUrl = "/uploads/products/TXKD-PET-455ml-chiet-lanh-1.jpg",
                StockQty = 100,
                ViewCount = 0,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now,
                ProductImages = new List<ProductImage>
                {
                    new() { Url = "/uploads/products/TXKD-PET-455ml-chiet-lanh-1.jpg", AltText = "Trà xanh Không Độ vị chanh chai 455ml", IsPrimary = true },
                    new() { Url = "/uploads/products/tra_xanh_khong_do_loc_6_chai_x_500_ml_62b73195851c42559a438ea15fcbc612_master.jpg", AltText = "Trà xanh Không Độ vị chanh chai 455ml", IsPrimary = false }
                }
            },
            new()
            {
                CategoryId = categories["Nước giải khát"].CategoryId,
                Name = "Nước tăng lực Number1 chai 330ml",
                Slug = "nuoc-tang-luc-number1-chai-330ml",
                BasePrice = 11300m,
                PromotionPrice = null,
                ShortDesc = "Nước tăng lực Number1 chai 330ml.",
                DetailDesc = "Nước tăng lực Number1 giúp giải khát và bổ sung năng lượng.",
                ThumbnailUrl = "/uploads/products/number1.jpg",
                StockQty = 100,
                ViewCount = 0,
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now,
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
