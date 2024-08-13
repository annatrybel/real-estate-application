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
            var messages = _context.Message.AsNoTracking().Where(m => !m.IsArchived).ToList();
            return View(messages);
        }

       
        public IActionResult CreateMessage([Bind("Name,Email,Phone,Messages")] WebApp.Models.Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

       

        [HttpPost]
        public IActionResult DeleteMessageFromArchive(int id)
        {
            var message = _context.Message.Find(id);
            if (message != null)
            {
                _context.Message.Remove(message);
                _context.SaveChanges();
            }

            return RedirectToAction("Archive");
        }

        [HttpPost]
        public IActionResult DeleteSelectedMessages(int[] selectedMessages)
        {
            if (selectedMessages != null && selectedMessages.Length > 0)
            {
                var messages = _context.Message.Where(m => selectedMessages.Contains(m.Id)).ToList();
                _context.Message.RemoveRange(messages);
                _context.SaveChanges();
            }
            return RedirectToAction("Index"); 
        }


        public IActionResult Archive()
        {
            var archivedMessages = _context.Message.Where(m => m.IsArchived).ToList();
            return View(archivedMessages); 
        }

        
        [HttpPost]
        public IActionResult ArchiveSelectedMessages(int[] selectedMessages)
        {
            if (selectedMessages != null && selectedMessages.Length > 0)
            {
                var messages = _context.Message.Where(m => selectedMessages.Contains(m.Id)).ToList();
                foreach (var message in messages)
                {
                    message.IsArchived = true;
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
