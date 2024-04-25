// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace CloudDevPOE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Inject IWebHostEnvironment into the controller's constructor
        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult MyWork()
        {
            return View();
        }

        // GET:
        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        // POST: Add product
        [HttpPost]
        public IActionResult AddProduct(Tbl_Products product, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        Console.WriteLine($"Error in property: {state.Key}, Error: {state.Value.Errors.First().ErrorMessage}");
                    }
                }
                return View(product);
            }

            // Safely get the user ID from the session
            int? userID = httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (!userID.HasValue)
            {
                // The user is not logged in, redirect them to the login page
                return RedirectToAction("Login", "User");
            }

            // Pass the webHostEnvironment to the Insert_Product method
            int rowsAffected = product.Insert_Product(product, userID.Value, _webHostEnvironment);
            if (rowsAffected > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}