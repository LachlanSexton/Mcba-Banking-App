using Microsoft.AspNetCore.Mvc;

namespace McbaExample.Controllers;


[Route("/Mcba/Login")]
public class LoginController : Controller
{


    public LoginController()
    {
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string loginID, string password)
    {
        if (loginID == null || string.IsNullOrEmpty(password) || loginID != "admin" || password != "admin")
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View();
        }

        return RedirectToAction("Admin", "Admin");
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
