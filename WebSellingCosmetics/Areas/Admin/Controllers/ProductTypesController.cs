using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebSellingCosmetics.Models;

namespace WebSellingCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, NhanVien")]
    public class ProductTypesController : Controller
    {
        private readonly WebMyPhamContext _context;
        public INotyfService _notyfService { get; }
        public ProductTypesController(WebMyPhamContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/ProductTypes
        public async Task<IActionResult> Index()
        {
            return _context.ProductTypes != null ?
                        View(await _context.ProductTypes.ToListAsync()) :
                        Problem("Entity set 'WebMyPhamContext.ProductTypes'  is null.");
        }

        // GET: Admin/ProductTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductTypes == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes
                .FirstOrDefaultAsync(m => m.ProductTypeId == id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // GET: Admin/ProductTypes/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType productType)
        {
            var exit = _context.ProductTypes.FirstOrDefault(m => m.Name == productType.Name);
            if (exit != null)
            {
                _notyfService.Error("Loại sản phẩm đã tồn tại");
                return View(productType);
            }

            _context.Add(productType);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm thành công");
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/ProductTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.ProductTypes == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
        
            return View(productType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductType productType)
        {


            try
            {
                var exit = _context.ProductTypes.FirstOrDefault(m =>m.ProductTypeId != productType.ProductTypeId && m.Name == productType.Name );
                if (exit != null)
                {
                    _notyfService.Error("Loại sản phẩm đã tồn tại");
                    return View(productType);
                }

                _notyfService.Success("Sửa thành công");
                _context.Update(productType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(productType.ProductTypeId))
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

        // GET: Admin/ProductTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductTypes == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes
                .FirstOrDefaultAsync(m => m.ProductTypeId == id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // POST: Admin/ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductTypes == null)
            {
                return Problem("Entity set 'WebMyPhamContext.ProductTypes'  is null.");
            }
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType != null)
            {
                _context.ProductTypes.Remove(productType);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypeExists(int id)
        {
            return (_context.ProductTypes?.Any(e => e.ProductTypeId == id)).GetValueOrDefault();
        }
    }
}
