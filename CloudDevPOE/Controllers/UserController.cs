// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
	public class UserController : Controller
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IConfiguration _configuration;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// Constructor to inject IHttpContextAccessor and IConfiguration
		public UserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
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
		[HttpGet]
		public ActionResult Login()
		{
			ViewBag.IsValidUser = true;
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
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