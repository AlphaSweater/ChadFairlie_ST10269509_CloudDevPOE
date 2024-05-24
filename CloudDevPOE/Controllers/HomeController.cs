// Ignore Spelling: Accessor

using CloudDevPOE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CloudDevPOE.Controllers
{
	public class HomeController : Controller
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly ILogger<HomeController> _logger;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public HomeController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger)
		{
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult Index()
		{
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult AboutUs()
		{
			return View();
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public IActionResult ContactUs()
		{
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