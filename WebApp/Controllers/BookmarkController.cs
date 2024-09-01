using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    [Authorize]
    public class BookmarkController : Controller
    {

        private readonly WebAppContext _context;

        public BookmarkController(WebAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Bookmarked> bookmarkList = new List<Bookmarked>();
            if(HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session)!=null
                && HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session).Count() > 0)
            {
                bookmarkList = HttpContext.Session.Get<List<Bookmarked>>(WC.Session);
            }
            
            List<int> prodInBookmark = bookmarkList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _context.Product
            .Include(u => u.Images)
            .Where(u => prodInBookmark.Contains(u.Id));

            return View(productList);            
        }


        public IActionResult Remove(int id) {
            List<Bookmarked> bookmarkList = new List<Bookmarked>();
            if (HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session) != null
                && HttpContext.Session.Get<IEnumerable<Bookmarked>>(WC.Session).Count() > 0)
            {
                bookmarkList = HttpContext.Session.Get<List<Bookmarked>>(WC.Session);
            }

            bookmarkList.Remove(bookmarkList.FirstOrDefault(i => i.ProductId == id));

            HttpContext.Session.Set(WC.Session, bookmarkList);

            return RedirectToAction(nameof(Index));
        }
    }
}
