using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem>? CategorySelectList { get; set; }
        public IEnumerable<SelectListItem>? ListingsTypeSelectList { get; set; }
        public IEnumerable<SelectListItem>? MarketSelectList { get; set; }
        public IEnumerable<SelectListItem>? BuildingTypeSelectList { get; set; }
    }
}