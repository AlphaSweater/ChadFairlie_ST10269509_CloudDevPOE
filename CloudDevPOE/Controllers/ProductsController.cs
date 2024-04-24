using CloudDevPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CloudDevPOE.Services;

namespace CloudDevPOE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ImageService _imageService;

        public ProductsController(ImageService imageService)
        {
            _imageService = imageService;
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
            if (ModelState.IsValid)
            {
                // Get the user ID from the session
                int? userID = httpContextAccessor.HttpContext.Session.GetInt32("UserId");
                if (userID == null)
                {
                    // The user is not logged in, redirect them to the login page
                    return RedirectToAction("Login", "User");
                }

                int rowsAffected = product.Insert_Product(product, userID.Value, _imageService);
                if (rowsAffected > 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        Console.WriteLine($"Error in property: {state.Key}, Error: {state.Value.Errors.First().ErrorMessage}");
                    }
                }
            }
            return View(product);
        }
    }
}