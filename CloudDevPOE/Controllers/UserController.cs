using Microsoft.AspNetCore.Mvc;
using CloudDevPOE.Models;

namespace CloudDevPOE.Controllers
{
    public class UserController : Controller
    {
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
            Tbl_Users userRepo = new Tbl_Users();
            bool isValidUser = userRepo.Validate_User(user);
            if (isValidUser)
            {
                return RedirectToAction("Index", "Home");
            } else
            {
                ViewBag.ErrorMessage = "Incorrect email or password.";
            }
            return View(user);
        }
    }
}