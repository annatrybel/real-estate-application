using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using WebApp.Utility;


namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebAppContext _context;

        public HomeController(ILogger<HomeController> logger, WebAppContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _context.Product.Include(p => p.Category).Include(p => p.ListingsType),
                Categories = _context.Category,
                ListingsType = _context.ListingsType
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<Bookmarked> bookmarkedList = new List<Bookmarked>();
            if (HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session) != null
                && HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session).Count() > 0)
            {
                bookmarkedList = HttpContext.Session.Get<List<Bookmarked>>(WC.Session);
            }

            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _context.Product.Include(p => p.Category).Include(p => p.ListingsType).Where(p => p.Id == id).FirstOrDefault(),
                ExistsInBookmarks = false
            };

            foreach (var item in bookmarkedList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistsInBookmarks = true;
                }
            }

            return View(detailsVM);
        }


        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {            
            List<Bookmarked> bookmarkedList = new List<Bookmarked>();
            if (HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session) != null
                && HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session).Count() > 0)
            {
                bookmarkedList = HttpContext.Session.Get<List<Bookmarked>>(WC.Session);
            }
            bookmarkedList.Add(new Bookmarked { ProductId = id });
            HttpContext.Session.Set(WC.Session, bookmarkedList);
            return RedirectToAction(nameof(Index)) ;
        }


      
        public IActionResult RemoveFromBookmarked(int id)
        {
            List<Bookmarked> bookmarkedList = new List<Bookmarked>();
            if (HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session) != null
                && HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session).Count() > 0)
            {
                bookmarkedList = HttpContext.Session.Get<List<Bookmarked>>(WC.Session);
            }
            var itemToRemove = bookmarkedList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                bookmarkedList.Remove(itemToRemove);
            }
                        
            HttpContext.Session.Set(WC.Session, bookmarkedList);
            return RedirectToAction(nameof(Index));
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
