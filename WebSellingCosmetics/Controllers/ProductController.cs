using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSellingCosmetics.Models;

namespace WebSellingCosmetics.Controllers
{
	public class ProductController : Controller
	{
		private readonly WebMyPhamContext _context;
		public INotyfService _notyfService { get; }
		public ProductController(WebMyPhamContext context, INotyfService notyfService)
		{

			_context = context;
			_notyfService = notyfService;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _context.Products.Include(x => x.ProductInventory).Include(x => x.ProductType).ToListAsync());
		}

        public async Task<IActionResult> SanPham(int id)
        {
            return View(await _context.Products.Include(x => x.ProductInventory).Where(x=>x.ProductTypeId == id).ToListAsync());
        }
        public async Task<IActionResult> Details(int id)
		{
			return View(await _context.Products.Include(x => x.ProductInventory).FirstOrDefaultAsync(x => x.ProductId == id));
		}
	}
}
