using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult HomeDecorPage()
        {
            return View();
        }
        public IActionResult JewelryPage()
        {
            return View();
        }
        public IActionResult MetalArtPage()
        {
            return View();
        }
        public IActionResult PotteryPage()
        {
            return View();
        }
        public IActionResult TextilesPage()
        {
            return View();
        }
        public IActionResult Woodwork()
        {
            return View();
        }
    }
}
