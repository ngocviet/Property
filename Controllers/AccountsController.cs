using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Property.Models;
using Property.Unitity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Property.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ProtalContext _context;

        public AccountsController(ProtalContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        [HttpGet]
        [Route("login.html", Name = "dangnhap")]
        [AllowAnonymous]
        public IActionResult login()
        {
            return View();
        }
        [HttpGet]
        [Route("register.html", Name = "dangky")]
        [AllowAnonymous]
        public IActionResult register()
        {
            return View();
        }
        [HttpPost]
        [Route("register.html", Name = "dangky")]
        [AllowAnonymous]
        public IActionResult register([Bind("AccountId,PackageId,Username,Password,Name,Address,Phone,Avatar,Permissions,NumberPost,Status")] Account account)
        {
            if (account.Username == null || account.Password == null || account.Phone == null || account.Name == null)
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }
            // Kiểm tra mật khẩu có 6 ký tự hay không
            if (account.Password.Length != 6)
            {
                ViewBag.ErrorMessage = "Mật khẩu phải có hơn 6 ký tự.";
                return View();
            }
            var listAcc = _context.Accounts.ToList();
            foreach (var item in listAcc)
            {
                if (item.Username.Equals(account.Username))
                {
                    ViewBag.ErrorMessage = "Tài khoản đã tồn tại.";
                    return View();
                }
            }
            account.Password = GetMd5Hash(account.Password);
            var list = _context.Packages.FirstOrDefault();
            account.PackageId = list.PackageId;
            account.Permissions = "User";
            account.NumberPost = 0;
            account.Avatar = "noimg.gif";
            _context.Add(account);
            _context.SaveChanges();
            ViewBag.SuccessMessage = "Đăng ký thành công!";
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "dangnhap")]
        public async Task<IActionResult> login(string username, string password)
        {
            if (username == null || password == null)
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }
            password = GetMd5Hash(password);
            var check = _context.Accounts.Where(v => v.Username.Equals(username) && v.Password.Equals(password) && v.Status == false).FirstOrDefault();
            if (check != null)
            {
                if (check.Permissions.Equals("Admin"))
                {
                    var check1 = HttpContext.Session.GetString("UserID");
                    if (check1 != null)
                    {
                        HttpContext.Session.Remove("UserID");
                        await HttpContext.SignOutAsync();
                    }
                    HttpContext.Session.SetString("AdminID", check.AccountId.ToString());
                }
                else
                {
                    var check1 = HttpContext.Session.GetString("AdminID");
                    if (check1 != null)
                    {
                        HttpContext.Session.Remove("AdminID");
                        await HttpContext.SignOutAsync();
                    }
                    HttpContext.Session.SetString("UserID", check.AccountId.ToString());
                }
                var claims = new List<Claim>
                {
                    new Claim("name", check.Name),
                    new Claim("avatar", check.Avatar),
                    new Claim("iduser", check.AccountId.ToString()),
                    new Claim("permission", check.Permissions)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if (check.Permissions.Equals("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.er = "v";
            }

            return View();
        }
        [Authenticated]
        public async Task<IActionResult> update([Bind("AccountId,PackageId,Username,Password,Name,Address,Phone,Avatar,Permissions,NumberPost,Status")] Account account)
        {
            account.Status = false;
            _context.Update(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Home", new {mss = "v"});
        }
        [Authenticated]
        [HttpGet]
        [Route("changepass.html", Name = "doimk")]
        public IActionResult changepass()
        {
            return View();
        }
        [Authenticated]
        [HttpPost]
        [Route("changepass.html", Name = "doimk")]
        public IActionResult changepass(string currentPass, string newPass)
        {
            currentPass = GetMd5Hash(currentPass);
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            Account account = _context.Accounts.Where(v => v.Status == false && v.AccountId == Int32.Parse(check.Value)).FirstOrDefault();

            if (currentPass.Equals(account.Password))
            {
                account.Password = GetMd5Hash(newPass);
                _context.Update(account);
                _context.SaveChanges();
                ViewBag.SuccessMessage = "Đăng ký thành công!";
            }
            else
            {
                ViewBag.ErrorMessage = "Mật khẩu cũ không chính xác.";
            }

            return View();
        }
        [Authenticated]
        [Route("logout.html", Name = "logoutuser")]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("login", "Accounts", new { area = "" });
        }
    }
}
