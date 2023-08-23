using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Property.Unitity;
using System.Security.Claims;
using System.Security.Principal;

namespace TestPro3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authenticated]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "permission").Value.Equals("Admin");
            if (check)
            {
                return View();
            }
            return RedirectToAction("login", "Accounts");
        }
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("login", "Accounts", new { area = "" });
        }
    }
}
