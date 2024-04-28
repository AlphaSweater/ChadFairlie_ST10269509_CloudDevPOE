// Ignore Spelling: Accessor

using Microsoft.AspNetCore.Mvc;
using CloudDevPOE.Models;
using Microsoft.AspNetCore.Http;

namespace CloudDevPOE.Controllers
{
	public class UserController : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserController(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
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
				int rowsAffected = user.Insert_User(user);
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
			int? userId = user.Validate_User(user);
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