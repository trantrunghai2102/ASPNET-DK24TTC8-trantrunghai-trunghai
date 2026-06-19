using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BanCaPheNuocGiaiKhat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(BanCaPheNuocGiaiKhat.Models.UserRoles.Admin))
                    return RedirectToAction("Index", "AdminDashboard");
                if (User.IsInRole(BanCaPheNuocGiaiKhat.Models.UserRoles.Staff))
                    return RedirectToAction("Index", "BanHang");
            }

            // Fetch 3 newest products
            var newProducts = await _db.Products
                .Where(p => p.Status == "active")
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .Select(p => new ProductCardViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    BasePrice = p.BasePrice,
                    PromotionPrice = p.PromotionPrice,
                    ShortDesc = p.ShortDesc,
                    ThumbnailUrl = p.ThumbnailUrl,
                    IsNew = true,
                    IsBestseller = false
                })
                .ToListAsync();

            // Fetch 1 best seller (most viewed)
            var bestSeller = await _db.Products
                .Where(p => p.Status == "active")
                .OrderByDescending(p => p.ViewCount)
                .Select(p => new ProductCardViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    BasePrice = p.BasePrice,
                    PromotionPrice = p.PromotionPrice,
                    ShortDesc = p.ShortDesc,
                    ThumbnailUrl = p.ThumbnailUrl,
                    IsNew = false,
                    IsBestseller = true
                })
                .FirstOrDefaultAsync();

            ViewBag.NewProducts = newProducts;
            ViewBag.BestSeller = bestSeller;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
