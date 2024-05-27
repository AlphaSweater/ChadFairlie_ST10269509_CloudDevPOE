// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace CloudDevPOE.Controllers
{
	public class HomeController : BaseController
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//

		private readonly ILogger<HomeController> _logger;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public HomeController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ILogger<HomeController> logger)
			: base(httpContextAccessor, webHostEnvironment, configuration)
		{
			_logger = logger;
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult Index()
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();			

			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult AboutUs()
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();

			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult ContactUs()
		{
			// Set the user details for the view and set the ViewData property
			ViewData["UserDetails"] = SetUserDetails();

			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}