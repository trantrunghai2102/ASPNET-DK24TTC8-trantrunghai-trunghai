using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BanCaPheNuocGiaiKhat.Models;

public class CategoryViewModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên danh mục")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Danh mục cha")]
    public int? ParentId { get; set; }

    public string? ParentName { get; set; }

    public int ProductCount { get; set; }

    // Dùng cho Dropdownlist
    public List<SelectListItem> ParentCategories { get; set; } = new();
}

public class CategoryListViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
}
