using CloudDevPOE.Models;
using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudDevPOE.Controllers
{
	public class BaseController : Controller
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly IConfiguration _configuration;

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// Inject IHttpContextAccessor, IWebHostEnvironment and IConfiguration into the controller's constructor
		public BaseController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		protected NavbarViewModel SetUserDetails()
		{
			// Safely get the user ID from the session
			int? userID = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
			if (userID.HasValue)
			{
				var connectionString = _configuration.GetConnectionString("DefaultConnection");
				Tbl_Users userModel = new Tbl_Users();

				NavbarViewModel navbar = userModel.GetNameAndCartCount(userID.Value, connectionString);

				return navbar;
			}
			return null;
		}
	}
}