using Microsoft.AspNetCore.Mvc;
using CloudDevPOE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CloudDevPOE.Controllers
{
	public class UserController : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;

		// Constructor to inject IConfiguration
		public UserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}

		// GET: Account/SignUp
		[HttpGet]
		public ActionResult SignUp()
		{
			return View();
		}

		// POST: Account/SignUp
		[HttpPost]
		public IActionResult SignUp(Tbl_Users user)
		{
			if (ModelState.IsValid)
			{
				var connectionString = _configuration.GetConnectionString("DefaultConnection");

				// Pass the connection string to Insert_User
				int rowsAffected = user.Insert_User(user, connectionString);
				if (rowsAffected > 0)
				{
					return RedirectToAction("Index", "Home");
				}
			}
			return View(user);
		}

		// GET: Account/Login
		[HttpGet]
		public ActionResult Login()
		{
			ViewBag.IsValidUser = true;
			return View();
		}

		// POST: Account/Login
		[HttpPost]
		public IActionResult Login(Tbl_Users user)
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			// Pass the connection string to Validate_User
			int? userId = user.Validate_User(user, connectionString);
			if (userId.HasValue)
			{
				_httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId.Value);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.ErrorMessage = "Incorrect email or password.";
				return View(user);
			}
		}

		// GET: Account/Logout
		[HttpGet]
		public IActionResult Logout()
		{
			_httpContextAccessor.HttpContext.Session.Clear();
			return RedirectToAction("Login", "User");
		}
	}
}