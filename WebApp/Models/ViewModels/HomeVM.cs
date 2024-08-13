namespace WebApp.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<ListingsType> ListingsType { get; set; }

		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}
