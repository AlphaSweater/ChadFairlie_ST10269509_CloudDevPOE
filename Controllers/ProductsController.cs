using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
