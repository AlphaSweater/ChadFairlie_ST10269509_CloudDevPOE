// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
	public class ProductsController : Controller
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly IConfiguration _configuration;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// Inject IHttpContextAccessor, IWebHostEnvironment and IConfiguration into the controller's constructor
		public ProductsController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public IActionResult MyWork()
		{
			SetUserDetails();
			var connectionString = _configuration.GetConnectionString("DefaultConnection");
			Tbl_Products productsModel = new Tbl_Products();

			// Fetch the list of product summaries
			List<ProductSummaryViewModel> productSummaries = productsModel.ListProducts(connectionString);

			// Pass the list to the MyWork view
			return View(productSummaries);
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public ActionResult AddProduct()
		{
			SetUserDetails();
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
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
			int rowsAffected = product.InsertProduct(product, userID.Value, _webHostEnvironment, connectionString);

			if (rowsAffected > 0)
			{
				return RedirectToAction("MyWork", "Products");
			}
			return View();
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public IActionResult ViewProduct(int id, string color)
		{
			SetUserDetails();
			var connectionString = _configuration.GetConnectionString("DefaultConnection");
			Tbl_Products productsModel = new Tbl_Products();

			ProductDetailsViewModel productDetails = productsModel.ViewProduct(id, connectionString);

			if (productDetails == null)
			{
				return NotFound();
			}

			productDetails.HighlightColor = color;
			return View(productDetails);
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpPost]
		public IActionResult AddToCart(int productId, int quantity)
		{
			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
			if (!userID.HasValue)
			{
				// The user is not logged in, return a JSON result
				return Json(new { success = false, message = "User is not logged in" });
			}

			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			Tbl_Carts cartsModel = new Tbl_Carts();
			int cartId = cartsModel.GetActiveCart(userID.Value, connectionString);

			Tbl_Cart_Items cartItemsModel = new Tbl_Cart_Items();
			cartItemsModel.AddItemToCart(cartId, productId, quantity, connectionString);

			return Json(new { success = true, message = "Product added to cart" });
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private void SetUserDetails()
		{
			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
			if (userID.HasValue)
			{
				var connectionString = _configuration.GetConnectionString("DefaultConnection");
				Tbl_Users userModel = new Tbl_Users();

				// Fetch the user details
				UserViewModel userDetails = userModel.GetUserDetails(userID.Value, connectionString);

				// Set the ViewData property
				ViewData["UserDetails"] = userDetails;
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}