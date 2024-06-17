using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebMyPham.Helper;
using WebSellingCosmetics.Models;
using System.Text.RegularExpressions;
using UnidecodeSharpFork;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace WebSellingCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, NhanVien")]
    public class ProductsController : Controller
    {
        private readonly WebMyPhamContext _context;
        public static string? image;
        public INotyfService _notyfService { get; }
        public ProductsController(WebMyPhamContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var webMyPhamContext = _context.Products.Include(p => p.ProductInventory).Include(p => p.ProductType);
            return View(await webMyPhamContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductInventory)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProductInventoryId"] = new SelectList(_context.ProductsInventorys, "ProductInventoryId", "ProductInventoryId");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile fAvatar, IFormFile fAvatar1, IFormFile fAvatar2, IFormFile fAvatar3)
        {
            var exit =await _context.Products.FirstOrDefaultAsync(x => x.Name == product.Name);
            if (exit != null)
            {
                ViewData["ProductInventoryId"] = new SelectList(_context.ProductsInventorys, "ProductInventoryId", "ProductInventoryId");
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "Name");
                _notyfService.Error("Tên sản phẩm đã tồn tại");
                return View(product);
            }
            try
            {
                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name)) + extennsion;
                    product.Image = await Utilities.UploadFile(fAvatar, @"Product", image.ToLower());
                }
                if (fAvatar1 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar1.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name + "1")) + extennsion;
                    product.Image1 = await Utilities.UploadFile(fAvatar1, @"Product", image.ToLower());
                }
                if (fAvatar2 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar2.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name + "2")) + extennsion;
                    product.Image2 = await Utilities.UploadFile(fAvatar2, @"Product", image.ToLower());
                }
                if (fAvatar3 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar3.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name +"3")) + extennsion;
                    product.Image3 = await Utilities.UploadFile(fAvatar3, @"Product", image.ToLower());
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));


        }
        public string ConvertToSlug(string? title)
        {
            // Chuyển đổi chuỗi sang không dấu
            string slug = title.Unidecode();

            // Xóa các ký tự không phải chữ cái, số, hoặc dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-zA-Z0-9\s-]", "");

            // Thay thế khoảng trắng bằng dấu gạch ngang
            slug = slug.Replace(" ", "-").Trim();

            // Xóa các dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            // Xóa dấu gạch ngang ở đầu và cuối chuỗi
            slug = slug.Trim('-');

            return slug;
        }
        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductInventoryId"] = new SelectList(_context.ProductsInventorys, "ProductInventoryId", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "Name");
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile fAvatar, IFormFile fAvatar1, IFormFile fAvatar2, IFormFile fAvatar3)
        {
            var exit = await _context.Products.FirstOrDefaultAsync(x => x.ProductId != product.ProductId && x.Name == product.Name);
            if (exit != null)
            {

                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "Name");
                _notyfService.Error("Tên sản phẩm đã tồn tại");
                return View(product);
            }

            try
            {
                var productex = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == product.ProductId);
                if (productex == null)
                {
                    return NotFound();

                }
                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name)) + extennsion;
                    productex.Image = await Utilities.UploadFile(fAvatar, @"Product", image.ToLower());
                }
                if (fAvatar1 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar1.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name + "1")) + extennsion;
                    productex.Image1 = await Utilities.UploadFile(fAvatar1, @"Product", image.ToLower());
                }
                if (fAvatar2 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar2.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name + "2")) + extennsion;
                    productex.Image2 = await Utilities.UploadFile(fAvatar2, @"Product", image.ToLower());
                }
                if (fAvatar3 != null)
                {
                    string extennsion = Path.GetExtension(fAvatar3.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(product.Name + "3")) + extennsion;
                    productex.Image3 = await Utilities.UploadFile(fAvatar3, @"Product", image.ToLower());
                }
                productex.Name = product.Name;
                productex.Description = product.Description;
                productex.Price = product.Price;
                productex.Status = product.Status;

                _notyfService.Success("Sửa thành công");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            //ViewData["ProductInventoryId"] = new SelectList(_context.ProductsInventorys, "ProductInventoryId", "ProductInventoryId", product.ProductInventoryId);
            //ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeId", product.ProductTypeId);

        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductInventory)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'WebMyPhamContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            _notyfService.Success("Xóa thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
