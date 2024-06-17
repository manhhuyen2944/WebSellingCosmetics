using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebSellingCosmetics.Models;

namespace WebSellingCosmetics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebMyPhamContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(ILogger<HomeController> logger, WebMyPhamContext context, INotyfService notyfService)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
        }

        public async Task<IActionResult> Index()
        {
            int pageNumber = 2; // Số trang cần lấy (ví dụ: trang thứ 2)
            int pageSize = 8; // Số lượng sản phẩm trên mỗi trang

            var products = await _context.Products
                .Include(x => x.ProductType) .Include(x => x.ProductInventory)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var products1 = await _context.Products.Include(x => x.ProductInventory).Include(x => x.ProductType).OrderByDescending(x => x.ProductId).Take(8).ToListAsync();

            ViewBag.GiamGia = products1;
            ViewBag.MoiNhat = products;
            return View(await _context.Products.Include(x => x.ProductInventory).Include(x => x.ProductType).Take(8).ToListAsync());
        }
        public async Task<IActionResult> NumBer()
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            var maKH = makhclaim?.Value;
            var number = await _context.OderItems.Where(x => x.Oder.AccountId == int.Parse(maKH) && x.Oder.Status == 1).ToListAsync();
            int? numberorder = number.Count();
            return Ok(numberorder);
        }

        public IActionResult Product()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Search( string searchTerm)
        {
            var searchResults = _context.Products
                .Where(x => x.Name.Contains(searchTerm))
                .ToList();

            return Ok(searchResults);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{

        //    //var soluong = _context.MonHoc.Include(a => a.Chuongs).ThenInclude(x => x.Bais.OrderBy(x => x.SoBai)).AsNoTracking().ToList();
        //    //ViewData["lstSubject"] = lstMon;

        //    base.OnActionExecuting(filterContext);
        //}
    }
}
