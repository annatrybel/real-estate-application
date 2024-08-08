namespace WebApp.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();
        }
        public Product Product { get; set; }
        public Message Message { get; set; }
        public bool ExistsInBookmarks { get; set; }
    }
}
