using BanCaPheNuocGiaiKhat.Models.Entities;

namespace BanCaPheNuocGiaiKhat.Models;

public class ProductCardViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? PromotionPrice { get; set; }
    public string? ShortDesc { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestseller { get; set; }
}

public class ProductListViewModel
{
    public List<ProductCardViewModel> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    public int? SelectedCategoryId { get; set; }
    public string? SearchQuery { get; set; }
    public string? SortBy { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 6;
    public int TotalItems { get; set; }
}

public class ProductDetailViewModel
{
    public Product Product { get; set; } = null!;
    public List<ProductCardViewModel> RelatedProducts { get; set; } = new();
}

public class AdminProductListViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int StockQty { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}

public class AdminProductListIndexViewModel
{
    public List<AdminProductListViewModel> Products { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? SearchQuery { get; set; }
}

public class AdminProductCreateViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? ShortDesc { get; set; }
    public string? DetailDesc { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? PromotionPrice { get; set; }
    public int StockQty { get; set; }
    public string Status { get; set; } = "active";
}

public class AdminProductEditViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? ShortDesc { get; set; }
    public string? DetailDesc { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? PromotionPrice { get; set; }
    public int StockQty { get; set; }
    public string Status { get; set; } = "active";

    public List<ProductImageViewModel> ExistingImages { get; set; } = new();
    public List<int>? DeleteImageIds { get; set; }
}

public class ProductImageViewModel
{
    public int ImageId { get; set; }
    public string Url { get; set; } = string.Empty;
}

