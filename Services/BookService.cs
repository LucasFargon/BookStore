using Microsoft.AspNetCore.Mvc;

namespace BookStore.Services
{
    public class BookService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
