using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class MessageController : Controller
    {
        private readonly WebAppContext _context;

        public MessageController(WebAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var messages = _context.Message.ToList();
            return View(messages);
        }


       
        public IActionResult CreateMessage([Bind("Name,Email,Phone,Messages")] WebApp.Models.Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Index);
        }

        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            var message = _context.Message.Find(id);
            if (message != null)
            {
                _context.Message.Remove(message);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
