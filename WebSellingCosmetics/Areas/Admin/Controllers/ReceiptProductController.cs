using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UnidecodeSharpFork;
using WebSellingCosmetics.Models;
using WebMyPham.Helper;

namespace WebSellingCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, NhanVien")]
    public class ReceiptProductController : Controller
    {
        private readonly WebMyPhamContext _context;
        public static string? image;
        public INotyfService _notyfService { get; }
        public ReceiptProductController(WebMyPhamContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }

        // GET: Admin/ReceiptProducts
        public async Task<IActionResult> Index()
        {
            var webMyPhamContext = _context.ReceiptProducts.Include(r => r.Product);
            return View(await webMyPhamContext.ToListAsync());
        }

        // GET: Admin/ReceiptProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReceiptProducts == null)
            {
                return NotFound();
            }

            var receiptProduct = await _context.ReceiptProducts
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReceiptProductId == id);
            if (receiptProduct == null)
            {
                return NotFound();
            }

            return View(receiptProduct);
        }

        // GET: Admin/ReceiptProducts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptProduct receiptProduct, IFormFile fAvatar)
        {
            var SanPham = _context.Products.Include(x => x.ProductInventory).FirstOrDefault(x => x.ProductId == receiptProduct.ProductId);
            if (SanPham == null)
            {
                return NotFound();
            }
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(ConvertToSlug(SanPham.Name + receiptProduct.CreateDay)) + extennsion;
                receiptProduct.Image = await Utilities.UploadFile(fAvatar, @"ReceiptProduct", image.ToLower());
            }
            SanPham.ProductInventory.Quantity = SanPham.ProductInventory.Quantity + receiptProduct.Quantity;
            receiptProduct.Status = 1;
            _context.Add(receiptProduct);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm thành công");
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
        // GET: Admin/ReceiptProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReceiptProducts == null)
            {
                return NotFound();
            }

            var receiptProduct = await _context.ReceiptProducts.FindAsync(id);
            if (receiptProduct == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", receiptProduct.ProductId);
            return View(receiptProduct);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReceiptProduct receiptProduct, IFormFile fAvatar)
        {
            try
            {

                var SanPham = _context.ReceiptProducts.Include(x => x.Product).ThenInclude(y => y.ProductInventory).FirstOrDefault(x => x.ProductId == receiptProduct.ProductId);
                if(SanPham == null)
                {
                    return NotFound();
                }
                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(ConvertToSlug(receiptProduct.Image)) + extennsion;
                    SanPham.Image = await Utilities.UploadFile(fAvatar, @"ReceiptProduct", image.ToLower());
                }
                SanPham.ProductId = receiptProduct.ProductId;
                SanPham.Quantity = receiptProduct.Quantity;
                SanPham.Price = receiptProduct.Price;
                SanPham.Status = receiptProduct.Status;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptProductExists(receiptProduct.ReceiptProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            _notyfService.Success("Sửa thành công");
            return RedirectToAction(nameof(Index));


        }

        // GET: Admin/ReceiptProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReceiptProducts == null)
            {
                return NotFound();
            }

            var receiptProduct = await _context.ReceiptProducts
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReceiptProductId == id);
            if (receiptProduct == null)
            {
                return NotFound();
            }

            return View(receiptProduct);
        }

        // POST: Admin/ReceiptProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReceiptProducts == null)
            {
                return Problem("Entity set 'WebMyPhamContext.ReceiptProducts'  is null.");
            }
            var receiptProduct = await _context.ReceiptProducts.FindAsync(id);
            if (receiptProduct != null)
            {
                _context.ReceiptProducts.Remove(receiptProduct);
            }
            _notyfService.Success("Xóa thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptProductExists(int id)
        {
            return (_context.ReceiptProducts?.Any(e => e.ReceiptProductId == id)).GetValueOrDefault();
        }
    }
}
