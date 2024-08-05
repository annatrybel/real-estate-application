using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApp;
using WebApp.Controllers;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;
using WebApp.Utility;

namespace Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsAViewResult_WithHomeVM()            //czy metoda Index zwraca prawid³owy widok
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

            context.Product.Add(new Product { Id = 1, Name = "TestProduct", Description = "TestDescription" });
            context.Category.Add(new Category { Id = 1, Name = "TestCategory", DisplayOrder=1 });
            context.ListingsType.Add(new WebApp.Models.ListingsType { Id = 1, Name = "TestListingsType" });
            context.SaveChanges();

            var controller = new HomeController(context);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HomeVM>(viewResult.ViewData.Model);  //czy model w ViewResult jest typu HomeVM
            Assert.Single(model.Products);
            Assert.Single(model.Categories);
            Assert.Single(model.ListingsType);
        }


        public void Details_ReturnsAViewResult_WithDetailsVM()
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

            context.Product.Add(new Product { Id = 1, Name = "TestProduct", Description = "TestDescription" });
            context.Category.Add(new Category { Id = 1, Name = "TestCategory", DisplayOrder = 1 });
            context.ListingsType.Add(new WebApp.Models.ListingsType { Id = 1, Name = "TestListingsType" });
            context.SaveChanges();

            // Utworzenie kontrolera i przypisanie pustej sesji
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.Session.Get<IEnumerable<Bookmarked>>(WC.Session)).Returns(new List<Bookmarked>());
            var controller = new HomeController(context);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
                        
            var result = controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<DetailsVM>(viewResult.ViewData.Model);

            Assert.NotNull(model.Product);
            Assert.Equal(1, model.Product.Id);
            Assert.False(model.ExistsInBookmarks);
        }


        public void DetailsPost_AddsProductToBookmarks_AndRedirectsToIndex()
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

            context.Product.Add(new Product { Id = 1, Name = "TestProduct", Description = "TestDescription" });

            context.SaveChanges();

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.Get<IEnumerable<Bookmarked>>(WC.Session)).Returns(new List<Bookmarked>());
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            var controller = new HomeController(context);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            
            var result = controller.DetailsPost(1);

            
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Sprawdzenie, czy produkt zosta³ dodany do zak³adek
            var bookmarkedList = mockSession.Object.Get<List<Bookmarked>>(WC.Session);
            Assert.Single(bookmarkedList);
            Assert.Equal(1, bookmarkedList.First().ProductId);
        }


        public void RemoveFromBookmarked_AndRedirectsToIndex()
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

            context.Product.Add(new Product { Id = 1, Name = "TestProduct", Description = "TestDescription" });
            context.SaveChanges();
                       
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

           
            var initialBookmarks = new List<Bookmarked>
            {
                new Bookmarked { ProductId = 1 },
                new Bookmarked { ProductId = 2 }
            };

            // Konfiguracja sesji, aby zwraca³a przyk³adowe zak³adki
            mockSession.Setup(s => s.Get<List<Bookmarked>>(WC.Session)).Returns(initialBookmarks);
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            var controller = new HomeController(context);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = controller.RemoveFromBookmarked(1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Sprawdzenie, czy zak³adki zosta³y zaktualizowane w sesji
            mockSession.Verify(s => s.Set(WC.Session, It.Is<List<Bookmarked>>(b => b.Count == 1 && b.All(bm => bm.ProductId != 1))), Times.Once);
        }
    }
}

