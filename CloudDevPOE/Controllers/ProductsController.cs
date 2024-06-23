// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.Services;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
	public class ProductsController : BaseController
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly IConfiguration _configuration;

		private readonly SearchService _searchService;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// Inject IHttpContextAccessor, IWebHostEnvironment and IConfiguration into the controller's constructor
		public ProductsController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, SearchService searchService)
			: base(httpContextAccessor, webHostEnvironment, configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
			_searchService = searchService;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public async Task<IActionResult> MyWork(string search = "")
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();

			var connectionString = _configuration.GetConnectionString("DefaultConnection");
			Tbl_Products productsModel = new Tbl_Products();

			List<ProductSummaryViewModel> productSummaries;

			if (string.IsNullOrWhiteSpace(search))
			{
				// Fetch all products if no search term is provided
				productSummaries = productsModel.ListProducts(connectionString);
			}
			else
			{
				// Perform search using Azure Cognitive Search
				var searchResults = await _searchService.SearchProductsAsync(search);
				productSummaries = searchResults.Select(doc =>
				{
					int searchId = int.Parse(doc["product_id"].ToString());

					var productSummary = productsModel.GetProductSummaryById(searchId, connectionString);

					return productSummary;
				}).ToList();
			}

			return View(productSummaries);
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public ActionResult AddProduct()
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();
			ViewData["userID"] = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

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
		public IActionResult ViewProduct(int id, string color = "--color-light-purple")
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();

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
		public async Task<IActionResult> AddToCart(int productId, int quantity)
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
			int cartId = await cartsModel.GetActiveCartIDAsync(userID.Value, connectionString);

			Tbl_Cart_Items cartItemsModel = new Tbl_Cart_Items();
			await cartItemsModel.AddItemToCartAsync(cartId, productId, quantity, connectionString);

			return Json(new { success = true, message = "Product added to cart" });
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public async Task<IActionResult> GetSuggestions(string query)
		{
			var suggestions = await _searchService.GetSuggestionsAsync(query);
			var suggestionResults = suggestions.Select(doc => new
			{
				Id = doc["product_id"].ToString(),
				Name = doc["name"].ToString(),
				Category = doc["category"].ToString(),
				Price = doc["price"].ToString(),
				Description = doc["description"].ToString()
			});

			return Json(suggestionResults);
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}