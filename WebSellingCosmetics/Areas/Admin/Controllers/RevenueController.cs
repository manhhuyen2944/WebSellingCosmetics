﻿using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using WebSellingCosmetics.Models;

namespace WebSellingCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RevenueController : Controller
    {
        private readonly WebMyPhamContext _context;
        public static string? image;
        public INotyfService _notyfService { get; }
        public RevenueController(WebMyPhamContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public async Task< IActionResult> Index()
        {
            var DonHang = await _context.Oders.Include(x => x.Account).ThenInclude(x => x.Addresses).Where(x=>x.Status != 1).ToListAsync();
            var DoanhThu = await _context.Oders.Where(x=>x.Status != 1 || x.Status != 5).ToListAsync();
            var DonHuy = await _context.Oders.Where(x => x.Status == 5).ToListAsync();

            decimal? doanthucuahang = DoanhThu.Sum(x => x.Total);


            ViewBag.DonHuy = DonHuy.Count();
            ViewBag.TongDonHang = DonHang.Count();
            ViewBag.TongTien = doanthucuahang;
            return View(DonHang);
        }
        public async Task<IActionResult> Search(DateTime TuNgay,DateTime DenNgay)
        {
            if (TuNgay > DenNgay)
            {
                _notyfService.Error("Ngày bắt đầu ko được lớn hơn ngày kết thúc");
                return View();
            }   
            if (TuNgay > DateTime.Now)
            {
                _notyfService.Error("Ngày bắt đầu ko được lớn hơn ngày hiện tại");
                return View();
            }    
            var DonHang = await _context.Oders.Include(x => x.Account).ThenInclude(x => x.Addresses)
                .Where(x => x.Status != 1 && x.CreateDay >= TuNgay && x.CreateDay <= DenNgay).ToListAsync();
            
            var DoanhThu = await _context.Oders.Where(x =>( x.Status != 1 || x.Status != 5 )&& x.CreateDay >= TuNgay && x.CreateDay <= DenNgay).ToListAsync();
            var DonHuy = await _context.Oders.Where(x => x.Status == 5 && x.CreateDay >= TuNgay && x.CreateDay <= DenNgay).ToListAsync();

            decimal? doanthucuahang = DoanhThu.Sum(x => x.Total);
            ViewBag.DonHuy = DonHuy.Count();
            ViewBag.TongDonHang = DonHang.Count();
            ViewBag.TongTien = doanthucuahang;

            return View(DonHang);
        }
        public IActionResult ExportExcelNgay(DateTime TuNgay, DateTime DenNgay)
        {
            try
            {
                if (TuNgay > DenNgay)
                {
                    _notyfService.Error("Ngày bắt đầu ko được lớn hơn ngày kết thúc");
                    return RedirectToAction("Index");
                }
                if (TuNgay > DateTime.Now)
                {
                    _notyfService.Error("Ngày bắt đầu ko được lớn hơn ngày hiện tại");
                    return RedirectToAction("Index");
                }
                var data = (from order in _context.Oders
                            where order.Status != 1 && order.Status != 5 &&(order.CreateDay >= TuNgay && order.CreateDay <= DenNgay)
                            select new
                            {
                                OrderId = order.OdersId,  // Sửa tên thuộc tính này
                                AccountId = order.AccountId,
                                Total = order.Total,
                                CreateDay = order.CreateDay,
                                AddressId = order.AddressId,
                                Status = order.Status,
                                // Thêm các thuộc tính khác mà bạn muốn bao gồm trong kết quả
                                OderItems = order.OderItems.Select(oi => new
                                {
                                    OderId = oi.OderId,
                                    ProductId = oi.ProductId,
                                    Quantity = oi.Quantity,
                                    Product = new
                                    {
                                        ProductId = oi.Product.ProductId,
                                        // Thêm các thuộc tính khác của Product mà bạn muốn bao gồm
                                    }
                                }).ToList()
                            }).ToList();

                if (data != null && data.Count() > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToConvertDataTable(data.ToList()));

                        using (MemoryStream stream = new MemoryStream())
                        {

                            wb.SaveAs(stream);
                            string fileName = $"DoanhThu_{DateTime.Now.ToString("dd/MM/yyyy")}.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocuments.spreadsheetml.sheet", fileName);
                        }

                    }

                }
                _notyfService.Error("Lỗi xuất file ");
            }
            catch (Exception)
            {
                _notyfService.Error("Xuất Excel Thất Bại!");
            }
            return RedirectToAction("Index");
        }  
        public IActionResult ExportExcel()
        {
            try
            {
                var data = (from order in _context.Oders
                            where order.Status != 1 && order.Status != 5
                            select new
                            {
                                OrderId = order.OdersId,  // Sửa tên thuộc tính này
                                AccountId = order.AccountId,
                                Total = order.Total,
                                CreateDay = order.CreateDay,
                                AddressId = order.AddressId,
                                Status = order.Status,
                                // Thêm các thuộc tính khác mà bạn muốn bao gồm trong kết quả
                                OderItems = order.OderItems.Select(oi => new
                                {
                                    OderId = oi.OderId,
                                    ProductId = oi.ProductId,
                                    Quantity = oi.Quantity,
                                    Product = new
                                    {
                                        ProductId = oi.Product.ProductId,
                                        // Thêm các thuộc tính khác của Product mà bạn muốn bao gồm
                                    }
                                }).ToList()
                            }).ToList();

                if (data != null && data.Count() > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToConvertDataTable(data.ToList()));

                        using (MemoryStream stream = new MemoryStream())
                        {

                            wb.SaveAs(stream);
                            string fileName = $"DoanhThu_{DateTime.Now.ToString("dd/MM/yyyy")}.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocuments.spreadsheetml.sheet", fileName);
                        }

                    }

                }
                _notyfService.Error("Lỗi xuất file ");
            }
            catch (Exception)
            {
                _notyfService.Error("Xuất Excel Thất Bại!");
            }
            return RedirectToAction("Index");
        }
        public DataTable ToConvertDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> englishToVietnamese = new Dictionary<string, string>
    {
        { "AccountId", "Tên khách hàng" },
        { "Total", "Tổng tiền" },
        { "CreateDay", "Ngày thanh toán" },
        { "AddressId", "Địa chỉ" },
        { "OderItems", "Sản Phẩm" },
        { "Status", "Trạng thái" }
    };

            List<string> columnsToExport = new List<string>(englishToVietnamese.Keys);

            foreach (PropertyInfo prop in propInfo)
            {
                if (columnsToExport.Contains(prop.Name))
                {
                    dt.Columns.Add(englishToVietnamese[prop.Name]);
                }
            }

            foreach (T item in items)
            {
                var values = new object[columnsToExport.Count];
                int j = 0;
                for (int i = 0; i < propInfo.Length; i++)
                {
                    if (columnsToExport.Contains(propInfo[i].Name))
                    {
                        if (propInfo[i].Name == "Status")
                        {
                            byte status = (byte)propInfo[i].GetValue(item, null);
                            string sta = Setstatus(status);
                            values[j] = sta;
                        }
                        else if (propInfo[i].Name == "AddressId")
                        {
                            int addressId = (int)propInfo[i].GetValue(item, null);
                            string address = GetApartmentCodeFromId(addressId);
                            values[j] = address;
                        }
                        else if (propInfo[i].Name == "AccountId")
                        {
                            int accountId = (int)propInfo[i].GetValue(item, null);
                            string acc = GetApartmentCodeFromId1(accountId);
                            values[j] = acc;
                        }
                        else if (propInfo[i].Name == "OderItems")
                        {
                            // Lấy danh sách các ProductId từ danh sách OderItems
                            var oderItems = propInfo[i].GetValue(item, null) as IEnumerable<object>;

                            if (oderItems != null)
                            {
                                var productIds = oderItems.Select(oi => oi.GetType().GetProperty("ProductId").GetValue(oi, null)).Cast<int>();

                                // Sử dụng phương thức getnameproduct cho mỗi ProductId
                                var productNames = productIds.Select(productId => getnameproduct(productId));
                                values[j] = string.Join(",", productNames);
                            }
                            else
                            {
                                values[j] = null; // hoặc giá trị mặc định phù hợp nếu danh sách null
                            }
                        }

                        else
                        {
                            values[j] = propInfo[i].GetValue(item, null);
                        }
                        j++;
                    }
                }
                dt.Rows.Add(values);
            }

            return dt;
        }
        private string GetApartmentCodeFromId(int apartmentId)
        {
            var code = _context.Addresses.FirstOrDefault(x => x.AddressId == apartmentId);

            if (code != null)
            {
                var diachi = code.Street + "," + code.Ward + "," + code.District + "," + code.City + "," + code.Country;
                return diachi;
            }
            else
            {
                return null; // hoặc giá trị mặc định phù hợp với yêu cầu của bạn
            }

        }
        private string GetApartmentCodeFromId1(int AccountId)
        {
            var code = _context.Accounts.FirstOrDefault(x => x.AccountId == AccountId);
           
            return code.FullName;

        }
        private string getnameproduct(int nameid)
        {
            var code = _context.Products.FirstOrDefault(x => x.ProductId == nameid);
           
            return code.Name;

        }
        private string Setstatus(byte AccountId)
        {
            if (AccountId == 2)
            {
                return "Chờ Admin Xác Nhận";
            }else if (AccountId == 3)
            {
                return "Đang vận chuyển";
            }else if (AccountId == 4)
            {
                return "Hoàn thành";
            }

            return "";

        }
    }
}
