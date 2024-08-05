using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Controllers;
using Xunit;

namespace Tests
{
    public class ListingsTypeControllerTests
    {
        private DbContextOptions<WebAppContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithListOfListingsTypes()
        {
            var options = CreateNewContextOptions();

            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                context.ListingsType.Add(new ListingsType { Id = 2, Name = "Type2" });
                await context.SaveChangesAsync();
            }

            using (var context = new WebAppContext(options))
            {
                var controller = new ListingsTypeController(context);

                var result = await controller.Index() as ViewResult;


                var model = Assert.IsAssignableFrom<IEnumerable<ListingsType>>(result.Model);
                Assert.Equal(2, model.Count());
            }
        }


        
        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            var options = CreateNewContextOptions();
            var controller = new ListingsTypeController(new WebAppContext(options));

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithListingsType()
        {
            var options = CreateNewContextOptions();
            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                await context.SaveChangesAsync();
            }

            using (var context = new WebAppContext(options))
            {
                var controller = new ListingsTypeController(context);

               
                var result = await controller.Details(1) as ViewResult;

                
                var model = Assert.IsAssignableFrom<ListingsType>(result.Model);
                Assert.Equal(1, model.Id);
                Assert.Equal("Type1", model.Name);
            }
        }

        [Fact]
        public async Task Create_ReturnsViewResult()
        {
            var options = CreateNewContextOptions();
            var controller = new ListingsTypeController(new WebAppContext(options));

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_PostValidListingsType_ReturnsRedirectToActionResult()
        {
            var options = CreateNewContextOptions();
            var controller = new ListingsTypeController(new WebAppContext(options));
            var newType = new ListingsType { Id = 1, Name = "NewType" };

           
            var result = await controller.Create(newType) as RedirectToActionResult;

           
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            using (var context = new WebAppContext(options))
            {
                var type = await context.ListingsType.FindAsync(1);
                Assert.NotNull(type);
                Assert.Equal("NewType", type.Name);
            }
        }

        [Fact]
        public async Task Edit_ReturnsNotFoundResult_WhenIdIsNull()
        {
            var options = CreateNewContextOptions();
            var controller = new ListingsTypeController(new WebAppContext(options));

            var result = await controller.Edit(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithListingsType()
        {
            var options = CreateNewContextOptions();
            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                await context.SaveChangesAsync();
            }

            using (var context = new WebAppContext(options))
            {
                var controller = new ListingsTypeController(context);

                var result = await controller.Edit(1) as ViewResult;

                var model = Assert.IsAssignableFrom<ListingsType>(result.Model);
                Assert.Equal(1, model.Id);
                Assert.Equal("Type1", model.Name);
            }
        }

        [Fact]
        public async Task Edit_PostValidListingsType_ReturnsRedirectToActionResult()
        {
            var options = CreateNewContextOptions();
            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                await context.SaveChangesAsync();
            }

            var controller = new ListingsTypeController(new WebAppContext(options));
            var updatedType = new ListingsType { Id = 1, Name = "UpdatedType" };

            var result = await controller.Edit(1, updatedType) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            using (var context = new WebAppContext(options))
            {
                var type = await context.ListingsType.FindAsync(1);
                Assert.Equal("UpdatedType", type.Name);
            }
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenIdIsNull()
        {
            var options = CreateNewContextOptions();
            var controller = new ListingsTypeController(new WebAppContext(options));

            var result = await controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }


        public async Task Delete_ReturnsViewResult_WithListingsType()
        {
            var options = CreateNewContextOptions();
            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                await context.SaveChangesAsync();
            }

            using (var context = new WebAppContext(options))
            {
                var controller = new ListingsTypeController(context);

                var result = await controller.Delete(1) as ViewResult;

                var model = Assert.IsAssignableFrom<ListingsType>(result.Model);
                Assert.Equal(1, model.Id);
                Assert.Equal("Type1", model.Name);
            }
        }


        public async Task DeletePost_ValidId_ReturnsRedirectToActionResult()
        {
            var options = CreateNewContextOptions();
            using (var context = new WebAppContext(options))
            {
                context.ListingsType.Add(new ListingsType { Id = 1, Name = "Type1" });
                await context.SaveChangesAsync();
            }

            var controller = new ListingsTypeController(new WebAppContext(options));

            var result = await controller.DeletePost(1) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            using (var context = new WebAppContext(options))
            {
                var type = await context.ListingsType.FindAsync(1);
                Assert.Null(type);
            }
        }
    }
}

