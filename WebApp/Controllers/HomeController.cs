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
        private readonly WebAppContext _context;

        public HomeController(WebAppContext context)
        {
            _context = context;
        }


        public IActionResult Index(int page = 1, int pageSize = 30, string category = "", decimal? maxPrice = null, string city = "", string listingType = "")
        {
            var productsQuery = _context.Product.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ListingsType)
                .Include(p => p.Images)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category);
            }
           
            if(category == "all")
            {
                productsQuery = _context.Product.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ListingsType)
                .Include(p => p.Images)
                .AsQueryable();
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(city))
            {
                productsQuery = productsQuery.Where(p => p.City.Contains(city));
            }

            if (!string.IsNullOrEmpty(listingType))
            {
                productsQuery = productsQuery.Where(p => p.ListingsType.Name == listingType);
            }

            int totalItems = productsQuery.Count();
            var products = productsQuery.Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            var homeVM = new HomeVM()
            {
                Products = products,
                Categories = _context.Category.AsNoTracking().ToList(),
                ListingsType = _context.ListingsType.AsNoTracking().ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
            var sql = productsQuery.ToQueryString();
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
                Product = _context.Product.Include(p => p.Category).Include(p => p.ListingsType).Include(p=>p.Images).Where(p => p.Id == id).FirstOrDefault(),
                Message = new WebApp.Models.Message(),
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


        public IActionResult CreateMessage([Bind("Name,Email,Phone,Messages")] WebApp.Models.Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
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




        [HttpGet]
        public IActionResult MortgagePayment(int? id)
        {
            var product = _context.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new MortgagePaymentVM
            {
                LoanAmount = product.Price,
                ProductId = product.Id
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult MortgagePayment(MortgagePaymentVM model)
        {
            if (ModelState.IsValid)
            {
                model.MonthlyPayment = CalculateMonthlyPayment(model.LoanAmount, model.InterestRate, model.LoanTerm);
            }
            return View(model);
        }

        private decimal CalculateMonthlyPayment(decimal loanAmount, double interestRate, int loanTerm)
        {
            var monthlyRate = (interestRate / 100) / 12;
            var numberOfPayments = loanTerm * 12;

            if (monthlyRate == 0)
            {
                return loanAmount / numberOfPayments;
            }

            var monthlyPayment = loanAmount * (decimal)((monthlyRate * Math.Pow(1 + monthlyRate, numberOfPayments)) / (Math.Pow(1 + monthlyRate, numberOfPayments) - 1));
            return monthlyPayment;
        }



        [HttpGet]
        public IActionResult CreditCalculator(int Id)
        {
            var model = new CreditCalculatorVM
            {
                ProductId = Id
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult CreditCalculator(CreditCalculatorVM model)
        {
            if (ModelState.IsValid)
            {
                var maxMonthlyPayment = model.MonthlyIncome - model.MonthlyExpenses;
                var loanAmount = model.LoanAmount;
                var interestRate = model.InterestRate;
                var loanTerm = model.LoanTerm;

                var monthlyRate = (decimal)(interestRate / 100 / 12);
                var numberOfPayments = loanTerm * 12;

                if (monthlyRate == 0)
                {
                    model.MonthlyPaymentLimit = maxMonthlyPayment * numberOfPayments;
                }
                else
                {
                    var monthlyRateDouble = (double)monthlyRate;
                    var maxMonthlyPaymentDouble = (double)maxMonthlyPayment;

                    model.MonthlyPaymentLimit = (decimal)((maxMonthlyPaymentDouble / monthlyRateDouble) *
                        (1 - Math.Pow(1 + monthlyRateDouble, -numberOfPayments)));
                }
            }
            return View(model);
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
