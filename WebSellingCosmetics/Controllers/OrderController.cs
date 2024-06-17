using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebSellingCosmetics.Models;
using WebSellingCosmetics.Models.ViewModel;
using WebSellingCosmetics.Services;
using WebSellingCosmetics.ViewModel;

namespace WebSellingCosmetics.Controllers
{
    public class OrderController : Controller
    {
        private readonly WebMyPhamContext _context;
        public INotyfService _notyfService { get; }
        private readonly IVnPayService _vnPayService;
        public OrderController(WebMyPhamContext context, INotyfService notyfService,IVnPayService vnPayService)
        {

            _context = context;
            _notyfService = notyfService;
           _vnPayService = vnPayService;
        }
        public async Task<IActionResult> Index()
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");

            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;
            var giohang =await _context.OderItems.Include(x => x.Product)
                .Include(x => x.Oder).ThenInclude(x => x.Account)
                .Where(x => x.Oder.AccountId == int.Parse(maKH) && x.Oder.Status == 1 && x.ProductId != null).ToListAsync();
            if (giohang == null)
            {
                _notyfService.Warning("Bạn chưa có sản phẩm nào trong giỏ hàng");

                return RedirectToAction("Index", "Home");

            }
            var addresses = _context.Addresses
                .Where(x => x.AccountId == int.Parse(maKH))
                .Select(x => new AddressViewModel
                {
                    AddressId = x.AddressId,
                    Street = x.Street,
                    Ward = x.Ward,
                    District = x.District,
                    City = x.City,
                    // Gán các thuộc tính khác mà bạn muốn hiển thị trong dropdown
                })
                .ToList();
            ViewData["AddressId"] = new SelectList(addresses, "AddressId", "FormattedAddress");
            ViewBag.Giohang = giohang;
            return View(giohang);
        }
        public async Task<IActionResult> AddToCart(int id, string strURL)
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");

            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;
            // Truy vấn đơn hàng chưa hoàn thành
            var dondathang = await _context.Oders.FirstOrDefaultAsync(x => x.AccountId == int.Parse(maKH) && x.Status == 1);

            if (dondathang == null)
            {
                // Nếu không tìm thấy đơn hàng chưa hoàn thành, tạo đơn hàng mới
                dondathang = new Oder
                {
                    AccountId = int.Parse(maKH),
                    Status = 1,

                };
                _context.Oders.Add(dondathang);
                await _context.SaveChangesAsync();
            }

            // Truy vấn chi tiết đơn hàng để kiểm tra sách
            var chitietdonthang = await _context.OderItems.FirstOrDefaultAsync(x => x.OderId == dondathang.OdersId && x.ProductId == id);

            if (chitietdonthang == null)
            {
                // Nếu sách chưa có trong chi tiết đơn hàng, thêm mới sách vào chi tiết đơn hàng
                var sach = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                chitietdonthang = new OderItem
                {
                    OderId = dondathang.OdersId,
                    ProductId = id,
                    Quantity = 1,
                };

                _context.OderItems.Add(chitietdonthang);
                await _context.SaveChangesAsync();

            }
            else
            {
                // Sách đã có trong chi tiết đơn hàng, tăng số lượng lên một đơn vị
                chitietdonthang.Quantity++;
                await _context.SaveChangesAsync();
            }
            _notyfService.Success("Thêm sản phẩm thành công ");
            return Redirect(strURL);

        }

        public async Task<IActionResult> BuyNow(int id)
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");

            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;
            // Truy vấn đơn hàng chưa hoàn thành
            var dondathang = await _context.Oders.FirstOrDefaultAsync(x => x.AccountId == int.Parse(maKH) && x.Status == 1);

            if (dondathang == null)
            {
                // Nếu không tìm thấy đơn hàng chưa hoàn thành, tạo đơn hàng mới
                dondathang = new Oder
                {
                    AccountId = int.Parse(maKH),
                    Status = 1,

                };
                _context.Oders.Add(dondathang);
                await _context.SaveChangesAsync();
            }

            // Truy vấn chi tiết đơn hàng để kiểm tra sách
            var chitietdonthang = await _context.OderItems.FirstOrDefaultAsync(x => x.OderId == dondathang.OdersId && x.ProductId == id);

            if (chitietdonthang == null)
            {
                // Nếu sách chưa có trong chi tiết đơn hàng, thêm mới sách vào chi tiết đơn hàng
                var sach = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                chitietdonthang = new OderItem
                {
                    OderId = dondathang.OdersId,
                    ProductId = id,
                    Quantity = 1,
                };

                _context.OderItems.Add(chitietdonthang);
                await _context.SaveChangesAsync();

            }
            else
            {
                // Sách đã có trong chi tiết đơn hàng, tăng số lượng lên một đơn vị
                chitietdonthang.Quantity++;
                await _context.SaveChangesAsync();
            }
            _notyfService.Success("Thêm sản phẩm thành công ");
            return RedirectToAction("Index");

        }

        public IActionResult UpdateOrder(int? id)
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;

            // Truy vấn đơn hàng chưa hoàn thành
            var dondathang = _context.OderItems
                .Include(x => x.Oder)
                .Where(x => x.Oder.AccountId == int.Parse(maKH) && x.Oder.Status == 1);

            OderItem? sanpham = dondathang.FirstOrDefault(x => x.ProductId == id);

            // Kiểm tra sản phẩm có tồn tại trong đơn hàng chưa hoàn thành
            if (sanpham?.Quantity == 1)
            {
                _context.OderItems.Remove(sanpham);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            if (sanpham != null)
            {
                sanpham.Quantity = sanpham.Quantity - 1;
                _context.SaveChanges(); // Lưu thay đổi số lượng vào cơ sở dữ liệu
            }
            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart()
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;

            // Truy vấn đơn hàng chưa hoàn thành
            var dondathang = _context.OderItems
                .Include(x => x.Oder)
                .Where(x => x.Oder.AccountId == int.Parse(maKH)
                && x.Oder.Status == 1);
            // Xóa hết các mục trong danh sách đơn đặt hàng
            _context.OderItems.RemoveRange(dondathang);
            _context.SaveChanges();
            _notyfService.Success("Xóa giỏ hàng thành công");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveProduct(int id)
        {
            var makhclaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (makhclaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }
            var maKH = makhclaim.Value;

            // Truy vấn đơn hàng chưa hoàn thành
            var dondathang = _context.OderItems
                .Include(x => x.Oder)
                .Where(x => x.Oder.AccountId == int.Parse(maKH) && x.Oder.Status == 1);

            OderItem? sanpham = dondathang.FirstOrDefault(x => x.ProductId == id);

            if (sanpham != null)
            {
                _context.OderItems.Remove(sanpham);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            if (dondathang == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetDiscount(string ma)
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(x => x.Code == ma);
            return Ok(discount);
        }
        public async Task<IActionResult> CheckOut(int tongtien, int address, string maGiamGia, int pay, int Shiper)
        {
            var makhClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");

            if (makhClaim == null)
            {
                _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                return RedirectToAction("Index", "Home");
            }

            var maKH = makhClaim.Value;
            var giohang = await _context.Oders
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.AccountId == int.Parse(maKH) && x.Status == 1);

            if (giohang == null)
            {
                return NotFound();
            }

           
            decimal? discountPercent = 0;

            if (!string.IsNullOrEmpty(maGiamGia))
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(x => x.Code == maGiamGia);

                if (discount == null)
                {
                    _notyfService.Error("Mã giảm giá không tồn tại");
                    return RedirectToAction("Index");
                }

                discountPercent = discount.DiscountPercent;
                giohang.DiscountId = discount.DiscountId;
            }

            var orderItems = _context.OderItems
                .Include(x => x.Product)
                    .ThenInclude(x => x.ProductInventory)
                .Where(x => x.OderId == giohang.OdersId)
                .ToList();

            foreach (var item in orderItems)
            {
                if ((item.Product.ProductInventory.QuantitySold + item.Quantity) > item.Product.ProductInventory.Quantity)
                {
                    var soluongsp = item.Product.ProductInventory.Quantity - item.Product.ProductInventory.QuantitySold;
                    _notyfService.Error($"{item.Product.Name} chỉ còn {soluongsp} sản phẩm");
                    return RedirectToAction("Index");
                }

                item.Product.ProductInventory.QuantitySold += item.Quantity;
            }

            await _context.SaveChangesAsync();

            int accountTypeId = giohang.Account?.AccountTypeId ?? 5;
            accountTypeId = (accountTypeId == 1) ? 0 : (accountTypeId == 4) ? 5 : accountTypeId;

            decimal ship = (Shiper == 1) ? 30000 : 45000;

            giohang.Total = tongtien - discountPercent - (tongtien / 100 * accountTypeId) + ship;

            giohang.Account.Point += 100;
            UpdateAccountType(giohang.Account);

            giohang.Status = 2;
            giohang.CreateDay = DateTime.Now;
            giohang.AddressId = address;

            Payment payment = new Payment
            {
                OdersId = giohang.OdersId,
                PaymentMethodsId = pay,
                Amount = giohang.Total,
                PaymentDate = DateTime.Now,
                Status = 1
            };

            Shipment shipment = new Shipment
            {
                OdersId = giohang.OdersId,
                ShippingMethodsId = Shiper,
                TrackingNumber = GenerateRandomString(20),
                Status = 1
            };

            _context.Payments.Add(payment);
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
            if (pay == 2)
            {
                var VnPayModel = new VnPaymentRequestModel
                {
                    Amount =(double)(giohang.Total),
                    CreatedDate = DateTime.Now,
                    Description = $"{giohang.Account.FullName}",
                    FullName = giohang.Account.FullName,
                    OrderId = giohang.OdersId

                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, VnPayModel));

            }
            _notyfService.Success("Đặt hàng thành công");
            return RedirectToAction("Index", "Home");
        }


        private void UpdateAccountType(Account account)
        {
            if (account.Point >= 1000)
            {
                account.AccountTypeId = 2;
            }
            else if (account.Point >= 3000)
            {
                account.AccountTypeId = 3;
            }
            else if (account.Point >= 5000)
            {
                account.AccountTypeId = 4;
            }
        }
        private string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }

        [Authorize]
        public async Task< IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode !=  "00")
            {
                var makhClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");

                if (makhClaim == null)
                {
                    _notyfService.Error("Vui lòng đăng nhập trước khi mua hàng");
                    return RedirectToAction("Index", "Home");
                }

                var maKH = makhClaim.Value;
                var giohang = await _context.Oders
                    .Include(x => x.Account).Include(x => x.Payments).Include(x => x.Shipments).OrderByDescending(x => x.CreateDay)
                    .FirstOrDefaultAsync(x => x.AccountId == int.Parse(maKH) && x.Status == 2);
                giohang.Total = null;
                giohang.CreateDay = null;
                giohang.AddressId = null;
                giohang.Discount = null;
                giohang.Status = 1;
              
                _context.Payments.RemoveRange(giohang.Payments);
                _context.Shipments.RemoveRange(giohang.Shipments);
                await _context.SaveChangesAsync();
                _notyfService.Error($"Lỗi thanh toán VNPAY: {response.VnPayResponseCode}");
                return RedirectToAction("Index", "Home");
            }

            _notyfService.Success("Thanh toán thành công");
            return RedirectToAction("Index", "Home");
        }
    }
}
