// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CloudDevPOE.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IConfiguration _configuration;

		// Inject IWebHostEnvironment into the controller's constructor
		public ProductsController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
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
		public IActionResult AddProduct(Tbl_Products product)
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
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
			if (!userID.HasValue)
			{
				// The user is not logged in, redirect them to the login page
				return RedirectToAction("Login", "User");
			}

			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Pass the webHostEnvironment and connectionString to the Insert_Product method
			int rowsAffected = product.Insert_Product(product, userID.Value, _webHostEnvironment, connectionString);

			if (rowsAffected > 0)
			{
				return RedirectToAction("MyWork", "Products");
			}
			return View();
		}
	}
}