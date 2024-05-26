// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
	public class UserController : BaseController
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IConfiguration _configuration;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// Constructor to inject IHttpContextAccessor and IConfiguration
		public UserController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
			: base(httpContextAccessor, webHostEnvironment, configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public ActionResult SignUp()
		{
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		[HttpPost]
		public IActionResult SignUp(Tbl_Users user)
		{
			if (ModelState.IsValid)
			{
				var connectionString = _configuration.GetConnectionString("DefaultConnection");

				// Pass the connection string to Insert_User
				int rowsAffected = user.InsertUser(user, connectionString);
				if (rowsAffected > 0)
				{
					return RedirectToAction("Index", "Home");
				}
			}
			return View(user);
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpPost]
		public IActionResult Login(Tbl_Users user)
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Pass the connection string to Validate_User
			int? userId = user.ValidateUser(user, connectionString);
			if (userId.HasValue)
			{
				_httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId.Value);
				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false, message = "Incorrect email or password." });
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		[HttpGet]
		public IActionResult Logout()
		{
			_httpContextAccessor.HttpContext.Session.Clear();
			return RedirectToAction("Index", "Home");
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task<IActionResult> UserProfile()
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();

			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
			if (userID.HasValue)
			{
				// Create the UserAccountViewModel
				UserAccountViewModel userDetails = new UserAccountViewModel();

				// Use the Tbl_Users model to get the user details
				Tbl_Users user = new Tbl_Users();
				userDetails = user.GetUserDetailsAsync(userID.Value, connectionString);

				return View(userDetails);
			}
			else
			{
				// TODO: FIx here
				return RedirectToAction("Index", "Home");
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public IActionResult CheckoutCart()
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

			// Use the Tbl_Carts model to checkout the active cart
			Tbl_Carts carts = new Tbl_Carts();

			carts.CheckoutCart(userID.Value, connectionString);

			decimal newTotal = 0;
			return RedirectToAction("UserProfile");
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult UpdateCartQuantity(int cartItemId, int quantity)
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

			Tbl_Cart_Items cartItems = new Tbl_Cart_Items();
			Tbl_Carts carts = new Tbl_Carts();

			cartItems.UpdateItemQuantity(cartItemId, quantity, connectionString);

			int activeCartId = carts.GetActiveCartID(userID.Value, connectionString);

			decimal newTotal = carts.GetCartTotal(activeCartId, connectionString);

			return Json(new { newTotal = newTotal.ToString("C", new System.Globalization.CultureInfo("en-ZA")) });
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult RemoveCartItem(int cartItemId)
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

			Tbl_Cart_Items cartItems = new Tbl_Cart_Items();
			Tbl_Carts carts = new Tbl_Carts();

			cartItems.RemoveItemFromCart(cartItemId, connectionString);

			int activeCartId = carts.GetActiveCartID(userID.Value, connectionString);

			decimal newTotal = carts.GetCartTotal(activeCartId, connectionString);

			return Json(new { newTotal = newTotal.ToString("C", new System.Globalization.CultureInfo("en-ZA")) });
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}