// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace CloudDevPOE.Controllers
{
	public class HomeController : Controller
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly ILogger<HomeController> _logger;

		private readonly IConfiguration _configuration;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public HomeController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
			_configuration = configuration;
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult Index()
		{
			SetUserDetails();
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult AboutUs()
		{
			SetUserDetails();
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult ContactUs()
		{
			SetUserDetails();
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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