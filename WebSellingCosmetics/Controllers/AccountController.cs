using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebSellingCosmetics.Models;
using WebMyPham.Extension;
using WebMyPham.Helper;

namespace WebSellingCosmetics.Controllers
{
    public class AccountController : Controller
    {

        private readonly WebMyPhamContext _context;
        public static string image;
        public INotyfService _notyfService { get; }
        public AccountController(WebMyPhamContext context, INotyfService notyfService)
        {

            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string user, string pass)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var password = pass.ToMD5();
            //var anh = _context.GiaoVien.ToList();
            // Kiểm tra tên đăng nhập và mật khẩu
            var account = await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(u => u.UserName == user && u.Password == password);

            if (account == null)
            {
                // Tên đăng nhập hoặc mật khẩu không đúng
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Index", "Home");
            }
            if (account?.RoleId == 1 || account?.RoleId == 2)
            {
                _notyfService.Error("Tài khoản của bạn là tài khoản Admin");
                return RedirectToAction("Index", "Home");
            }
            if (account?.Status == 2)
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
                        new Claim("UserName" , account.UserName),
                        new Claim("Id" , account.AccountId.ToString()),
                         new Claim("Avartar", "/contents/Images/User/" + account.Avartar) // Thêm đường dẫn đến ảnh đại diện vào claims
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
            return RedirectToAction("Index", "Home");
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
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Account account)
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
            var exaccount = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email || x.UserName == account.UserName);
            if (exaccount != null)
            {
                _notyfService.Error("Email hoặc Username đã tồn tại");
                return View(account);
            }
            account.Avartar = "UserDemo.jpg";
            account.Password = (account.Password)?.ToMD5();
            account.Status = 1;
            account.AccountTypeId = 1;
            account.Point = 0;
            account.RoleId = 3;

            _context.Update(account);
            await _context.SaveChangesAsync();
            _notyfService.Success("Đăng ký thành công");
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Create(string streest, string ward, string distrist, string city, string contry , string url)
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;
            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }

            Address address = new Address
            {
                Street = streest,
                City = city,
                Ward = ward,
                District = distrist,
                Country = contry,
                AccountId = Id
            };
            _context.Update(address);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm địa chỉ thành công");
            if (url != null)
            {
                return Redirect(url);
            }
            return RedirectToAction("Profile", "Account");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int addresid, string streest, string ward, string distrist, string city, string contry)
        {
            var addresr = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addresid);
            if (addresr == null)
            {
                return NotFound();
            }
            addresr.Street = streest;
            addresr.City = city;
            addresr.Ward = ward;
            addresr.District = distrist;
            addresr.Country = contry;
            await _context.SaveChangesAsync();
            _notyfService.Success("Sửa địa chỉ thành công");
            return RedirectToAction("Profile", "Account");
        }

      
        [HttpPost]
        public async Task<IActionResult> DeleteAd(int addid)
        {
            var addresr = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addid);
            if (addresr == null)
            {
                return NotFound();
            }
            _context.Addresses.Remove(addresr);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa địa chỉ thành công");
            return RedirectToAction("Profile", "Account");
        }
        public async Task<IActionResult> Profile()
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;
            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }
            var donhang =await _context.Oders.Where(x=>x.AccountId == Id)
                .Include(x=>x.OderItems).ThenInclude(x=>x.Product).Include(x=>x.Account).ThenInclude(x=>x.Addresses).ToListAsync();
            if (donhang == null)
            {
                return NotFound();  
            }
            ViewBag.Donhang = donhang;
            ViewBag.Addresses = await _context.Addresses.Where(x => x.AccountId == Id).ToArrayAsync();


            return View(await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id));
        }

        public async Task<IActionResult> EditProfile()
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;
            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }

            return View(await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id));
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(Account account, IFormFile fAvatar)
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
            var khachang = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
            if (khachang == null)
            {
                return NotFound();
            }
            var ktemail = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId != account.AccountId
                && (x.Email == account.Email));
            if (ktemail != null)
            {
                _notyfService.Error("Email đã tồn tại trong hệ thống!");
                return View(khachang);
            }
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(khachang.UserName) + extennsion;
                khachang.Avartar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
            }
         

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Lưu thông tin người dùng vào cookie xác thực
            List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        new Claim("UserName" , account.UserName),
                        new Claim("Id" , account.AccountId.ToString()),
                         new Claim("Avartar", "/contents/Images/User/" + khachang.Avartar) // Thêm đường dẫn đến ảnh đại diện vào claims
                    };
            //   Response.Cookies.Append("AnhDaiDien", "Images/GiaoVien/" + user.AnhDaiDien);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            khachang.Email = account.Email;
            khachang.FullName = account.FullName;
            khachang.Gender = account.Gender;
            khachang.Phone = account.Phone;
            khachang.Birthday = account.Birthday;

            _notyfService.Success("Sửa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> CanOrderby(int id)
        {
            var oder = await _context.Oders.FirstOrDefaultAsync(x => x.OdersId == id);
            if (oder == null)
            {
                return NotFound();
            }
            oder.Status = 5;
            await _context.SaveChangesAsync();
            _notyfService.Success("Hủy đơn thành công");
            return RedirectToAction("Profile");
        }
    }
}
