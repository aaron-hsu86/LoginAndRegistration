using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using _4_LoginAndRegistration.Models;

namespace _4_LoginAndRegistration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext db;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        else 
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("Dashboard");
        }
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userCheck)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        else
        {
            User? userInDb = db.Users.FirstOrDefault(u => u.Email == userCheck.EmailCheck);
            if (userInDb == null)
            {
                ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
                ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

            var result = hasher.VerifyHashedPassword(userCheck, userInDb.Password, userCheck.PasswordCheck);
            if(result == 0)
            {
                ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
                ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Dashboard");
        }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [SessionCheck]
    [HttpGet("success")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}