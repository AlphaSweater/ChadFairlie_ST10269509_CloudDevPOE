using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult MyWork()
        {
            return View();
        }
    }
}