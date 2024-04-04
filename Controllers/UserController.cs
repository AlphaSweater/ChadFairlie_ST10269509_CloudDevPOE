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
        public IActionResult SignUp(tbl_users user)
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
    }
}