using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using WebSellingCosmetics.Models;
using WebMyPham.Extension;
using WebMyPham.Helper;
using Microsoft.AspNetCore.Authorization;

namespace WebSellingCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private WebMyPhamContext _context; public static string? image;
        public INotyfService _notyfService { get; }
        private readonly IConfiguration _configuration;
        public AccountController(WebMyPhamContext repo, INotyfService notyfService, IConfiguration configuration)
        {
            _context = repo;
            _notyfService = notyfService;
            _configuration = configuration;
        }
        [Authorize(Roles = "Admin, NhanVien")]
        [Route("Admin/Account/Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Accounts.Include(x => x.Addresses).Include(x => x.AccountType).Where(x => x.RoleId == 3).ToListAsync());
        }
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin, NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account, IFormFile fAvatar)
        {
            if (account.UserName?.Length < 6)
            {
                _notyfService.Error("Tài khoản không bé hơn 6 kí tự");
                return View(account);
            }
            if (account.Password?.Length < 6)
            {
                _notyfService.Error("Mật khẩu không bé hơn 6 kí tự");
                return View(account);
            }
            if (account.Phone?.Length != 10)
            {
                _notyfService.Error("Số điện thoại là 10 số");
                return View(account);
            }
            var mk = "123123";
            var mailex =await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email || x.UserName == account.UserName);
            if (mailex != null)
            {
                _notyfService.Error("Email hay tài khoản đã tồn tại");
                return View(account);
            }
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(account.UserName) + extennsion;
                account.Avartar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
            }
            account.Avartar = "UserDemo.jpg";
            account.Password = mk.ToMD5();
            account.AccountTypeId = 1;
            account.Point = 0;
            account.RoleId = 3;
            account.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(account.FullName);
            _context.Add(account);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm thành công");
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var acc = await _context.Accounts.Include(x => x.AccountType).Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == id);
            if (acc == null)
            {
                return NotFound();
            }

            return View(acc);
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var acc = await _context.Accounts.Include(x => x.AccountType).Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == id);
            if (acc == null)
            {
                return NotFound();
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "AccountTypeId", "Name");
            return View(acc);
        }
        [Authorize(Roles = "Admin, NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account, IFormFile fAvatar)
        {
            try
            {
                var cudanUp = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
                if (cudanUp == null)
                {
                    return NotFound();
                }

                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(account.UserName) + extennsion;
                    cudanUp.Avartar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                }
                else
                {
                    account.Avartar = _context.Accounts.Where(x => x.AccountId == account.AccountId).Select(x => x.Avartar).FirstOrDefault();
                }          
                cudanUp.FullName = account.FullName;
                cudanUp.Email = account.Email;
                cudanUp.Birthday = account.Birthday;
                cudanUp.Gender = account.Gender;
                cudanUp.Status = account.Status;
                cudanUp.AccountTypeId = account.AccountTypeId;
                cudanUp.UserName = account.UserName;
                var ktemail = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId != account.AccountId && (x.Email == account.Email && x.UserName == account.UserName));
                if (ktemail != null)
                {
                    ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "AccountTypeId", "Name");
                    _notyfService.Error("Email hay tên đăng nhập đã tồn tại trong hệ thống!");
                    return View(account);
                }
                _notyfService.Success("Sửa thành công!");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountsExists(account.AccountId))
                {
                    _notyfService.Error("Lỗi!!!!!!!!!!!!");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var acc = await _context.Accounts.Include(x => x.AccountType).Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == id);
            if (acc == null)
            {
                return NotFound();
            }

            return View(acc);
        }
        [Authorize(Roles = "Admin, NhanVien")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'WebMyPhamContext.Accounts'  is null.");
            }
            var product = await _context.Accounts.FindAsync(id);
            if (product != null)
            {
                _context.Accounts.Remove(product);
            }
            _notyfService.Success("Xóa thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountsExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// [Authorize(Roles = "Admin")]
        /// 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            return View(await _context.Accounts.Include(x => x.Addresses).Include(x => x.Role).Where(x => x.RoleId != 3).ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateQL()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Name");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQL(Account account, IFormFile fAvatar)
        {
            var mk = "123123";
            var mailex = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email || x.UserName == account.UserName);
            if (mailex != null)
            {
                _notyfService.Error("Email hay tài khoản đã tồn tại");
                return View(account);
            }
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(account.UserName) + extennsion;
                account.Avartar = await Utilities.UploadFile(fAvatar, @"Admin", image.ToLower());
            }
            account.Password = mk.ToMD5();

            account.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(account.FullName);
            _context.Add(account);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm thành công");
            return RedirectToAction("Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQL(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var acc = await _context.Accounts.Include(x => x.AccountType).Include(x => x.Role).FirstOrDefaultAsync(x => x.AccountId == id);
            if (acc == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Name");
            return View(acc);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQL(Account account, IFormFile fAvatar)
        {
            try
            {
                var cudanUp = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
                if (cudanUp == null)
                {
                    return NotFound();
                }

                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(account.UserName) + extennsion;
                    cudanUp.Avartar = await Utilities.UploadFile(fAvatar, @"Admin", image.ToLower());
                }
                else
                {
                    account.Avartar = _context.Accounts.Where(x => x.AccountId == account.AccountId).Select(x => x.Avartar).FirstOrDefault();
                }
                cudanUp.FullName = account.FullName;
                cudanUp.Email = account.Email;
                cudanUp.Birthday = account.Birthday;
                cudanUp.Gender = account.Gender;
                cudanUp.Status = account.Status;
                cudanUp.AccountTypeId = account.AccountTypeId;
                cudanUp.UserName = account.UserName;
                var ktemail = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId != account.AccountId && (x.Email == account.Email && x.UserName == account.UserName));
                if (ktemail != null)
                {
                    ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "AccountTypeId", "Name");
                    _notyfService.Error("Email hay tên đăng nhập đã tồn tại trong hệ thống!");
                    return View(account);
                }
                _notyfService.Success("Sửa thành công!");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountsExists(account.AccountId))
                {
                    _notyfService.Error("Lỗi!!!!!!!!!!!!");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Manage");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQL(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var acc = await _context.Accounts.Include(x => x.AccountType).Include(x => x.Role).FirstOrDefaultAsync(x => x.AccountId == id);
            if (acc == null)
            {
                return NotFound();
            }

            return View(acc);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("DeleteQL")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedQL(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'WebMyPhamContext.Accounts'  is null.");
            }
            var product = await _context.Accounts.FindAsync(id);
            if (product != null)
            {
                _context.Accounts.Remove(product);
            }
            _notyfService.Success("Xóa thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///3
        public async Task<IActionResult> Login()
        {

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(string user, string pass)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var password = pass.ToMD5();
 
            // Kiểm tra tên đăng nhập và mật khẩu
            var account = await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(u => u.UserName == user && u.Password == password);

            if (account == null)
            {
                // Tên đăng nhập hoặc mật khẩu không đúng
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Index", "Home");
            }
            if (account?.RoleId == 3)
            {
                _notyfService.Error("Tài khoản của bạn là tài khoản người dùng");
                return RedirectToAction("Index", "Home");
            }
            if (account.Status == 2)
            {
                _notyfService.Error("Tài khoản đã bị khóa");
                return RedirectToAction("Index", "Home");
            }
            if (account != null)
            {
                // Lưu thông tin người dùng vào cookie xác thực
                List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        account.RoleId == 1 ? new Claim(ClaimTypes.Role, "Admin") : new Claim(ClaimTypes.Role, "NhanVien"),
                        new Claim("UserName" , account.UserName),
                        new Claim("Id" , account.AccountId.ToString()),
                         new Claim("Avartar", "/contents/Images/Admin/" + account.Avartar) // Thêm đường dẫn đến ảnh đại diện vào claims
                    };
                //   Response.Cookies.Append("AnhDaiDien", "Images/GiaoVien/" + user.AnhDaiDien);
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                _notyfService.Success("Đăng nhập thành công");
                // Chuyển hướng đến trang chủ
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _notyfService.Warning("Tên đăng nhập hoặc mật khẩu không đúng");
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string pass, string newpass, string confirmpass)
        {
            if (ModelState.IsValid)
            {
                var tendangnhapclam = User.Claims.SingleOrDefault(c => c.Type == "UserName");
                var tendangnhap = "";
                if (tendangnhapclam != null)
                { tendangnhap = tendangnhapclam.Value; }
                var password = pass.ToMD5();
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == tendangnhap);
                if (user?.Password != password)
                {
                    _notyfService.Error("Mật khẩu cũ không chính xác");
                    return RedirectToAction("Index", "Home");
                }
                if (newpass.Length < 6 && newpass.Length < 100)
                {
                    _notyfService.Error("Mật khẩu mới phải trên 6 ký tự và nhỏ hơn 100 ký tự ");
                    return RedirectToAction("Index", "Home");
                }
                if (newpass != confirmpass)
                {
                    _notyfService.Error("Mật khẩu mới không đúng với mật khẩu xác nhận !");
                    return RedirectToAction("Index", "Home");
                }
                user.Password = newpass.ToMD5();
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                _notyfService.Error("Vui lòng nhập đầy đủ thông mật khẩu !");

            }
            _notyfService.Success("Đổi mật khẩu thành công!");
            return RedirectToAction("Index", "Home");
        }

    }
}
