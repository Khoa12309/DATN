using APPDATA.DB;
using APPDATA.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using static _APPAPI.Controllers.ChartController;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ShoppingDB _context;
        private readonly HttpClient _httpClient;

        public ChartController()
        {
            _context = new ShoppingDB(new DbContextOptions<ShoppingDB>());
            _httpClient = new HttpClient();
        }
        //Lấy dữ liệu thống kê theo năm
        [HttpGet("{year}")]
        public IActionResult StatisticsByYear(int year)
        {
            try
            {
                var data = _context.Bills
                    .Where(b => b.PayDate.HasValue && b.PayDate.Value.Year == year && b.Status == 4)
                    .GroupBy(b => b.PayDate.Value.Month)  // Nhóm hóa đơn theo tháng
                    .Select(group => new
                    {
                        Month = group.Key,
                        Money = group.Sum(b => b.TotalMoney)
                    })
                    .OrderBy(item => item.Month)
                    .ToList();

                // Chuyển đổi dữ liệu thành định dạng phù hợp cho biểu đồ
                var chartData = new
                {
                    labels = data.Select(item => item.Month),
                    values = data.Select(item => item.Money),
                    label = "Doanh Thu"
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // API để vẽ biểu đồ thống kê doanh thu trong 1 tuần qua
        [HttpGet("weekly")]
        public IActionResult WeeklyStatistics()
        {
            try
            {
                // Lấy ngày hiện tại
                DateTime currentDate = DateTime.Now;

                // Lấy tất cả các ngày trong tuần qua
                var allDatesInWeek = Enumerable.Range(0, 7)
                    .Select(offset => currentDate.AddDays(-offset).Date)
                    .ToList();

                // Lấy dữ liệu từ database cho doanh thu trong 1 tuần qua
                var data = _context.Bills
                    .Where(b => b.PayDate.HasValue && b.PayDate.Value >= currentDate.AddDays(-7) && b.Status == 4)
                    .GroupBy(b => b.PayDate.Value.Date)  // Nhóm theo ngày
                    .OrderBy(group => group.Key)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Money = group.Sum(b => b.TotalMoney)
                    })
                    .ToList();

                // Đảm bảo rằng tất cả các ngày trong tuần đều có dữ liệu
                var mergedData = allDatesInWeek
                    .GroupJoin(data, date => date, group => group.Date, (date, group) => new
                    {
                        Date = date,
                        Money = group.Sum(b => b?.Money ?? 0)
                    })
                    .OrderBy(item => item.Date)
                    .ToList();

                // Chuyển đổi dữ liệu thành định dạng phù hợp cho biểu đồ
                var chartData = new
                {
                    labels = mergedData.Select(item => item.Date.ToShortDateString()), // Format ngày thành chuỗi
                    values = mergedData.Select(item => item.Money),
                    label = "Doanh Thu Tuần Qua"
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("DatePicker")]
        public IActionResult StatisticsByDatePicker(DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = _context.Bills
                    .Where(b => b.PayDate.HasValue && b.PayDate >= startDate && b.PayDate <= endDate && b.Status == 4)
                    .GroupBy(b => b.PayDate.Value.Date)  // Nhóm hóa đơn theo tháng
                    .Select(group => new
                    {
                        Dates = group.Key,
                        Moneys = group.Sum(b => b.TotalMoney)
                    })
                    .OrderBy(item => item.Dates)
                    .ToList();

                // Chuyển đổi dữ liệu thành định dạng phù hợp cho biểu đồ
                var chartData = new
                {
                    labels = data.Select(item => item.Dates),
                    values = data.Select(item => item.Moneys),
                    label = "Doanh Thu trong năm"
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private ApiData GetDataFromApi(string apiUrl)
        {
            try
            {
                // Gọi API để lấy dữ liệu
                var response = _httpClient.GetAsync(apiUrl).Result;

                // Kiểm tra xem yêu cầu đã thành công hay không
                if (response.IsSuccessStatusCode)
                {
                    // Đọc nội dung từ phản hồi
                    var content = response.Content.ReadAsStringAsync().Result;

                    // Deserializing JSON thành danh sách DataItem (sử dụng thư viện Newtonsoft.Json)
                    var apiData = JsonConvert.DeserializeObject<ApiData>(content);

                    return apiData;
                }
                else
                {
                    throw new Exception($"API request failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác nếu có
                throw new Exception("Error calling API: " + ex.Message);
            }
        }


        public class ApiData
        {
            public List<string> Labels { get; set; }
            public List<decimal> Values { get; set; }
            public string Label { get; set; }
        }
        [HttpGet("ExportDataToExcel/{label}")]
        public IActionResult ExportDataToExcel(string? currentyear, string label, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Tạo một tệp Excel
                var excelPackage = new ExcelPackage();
                var worksheet = excelPackage.Workbook.Worksheets.Add("ChartData");
                string fileName = "";
                ApiData apiData = null;  // Đặt biến ở ngoài để tránh xung đột

                if (label == "yearly")
                {
                    fileName = "_Doanh_thu_" + currentyear ;
                    apiData = GetDataFromApi($"https://localhost:7042/api/Chart/" + currentyear);
                    worksheet.Cells["A1"].Value = "STT";    
                    worksheet.Cells["B1"].Value = "Tháng";
                    worksheet.Cells["C1"].Value = "Doanh thu";
                }
                else if (label == "weekly")
                {
                    fileName = "_7_ngay_gan_nhat";
                    apiData = GetDataFromApi($"https://localhost:7042/api/Chart/weekly");
                    worksheet.Cells["A1"].Value = "STT";
                    worksheet.Cells["B1"].Value = "Ngày";
                    worksheet.Cells["C1"].Value = "Doanh thu";
                }
                else if (label == "DatePicker")
                {
                    fileName = "_Doanh_thu_datepicker";
                    apiData = GetDataFromApi($"https://localhost:7042/api/Chart/DatePicker?startDate={startDate}&endDate={endDate}");
                    worksheet.Cells["A1"].Value = "STT";
                    worksheet.Cells["B1"].Value = "Ngày";
                    worksheet.Cells["C1"].Value = "Doanh thu";
                }
                else
                {
                    // Xử lý trường hợp không hỗ trợ
                    return BadRequest(new { error = "Invalid label specified" });
                }

                // Điền dữ liệu vào tệp Excel từ dữ liệu API

                for (int i = 0; i < apiData.Labels.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = i + 1;


                    if (label == "weekly")
                    {
                        worksheet.Cells[i + 2, 2].Value = apiData.Labels[i];
                    }
                    else
                    {
                        worksheet.Cells[i + 2, 2].Value = apiData.Labels[i] + "/2023";
                    }
                    worksheet.Cells[i + 2, 3].Value = apiData.Values[i];

                }

                // Lưu tệp Excel vào một MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Trả về tệp Excel như là một phản hồi HTTP
                    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}_Chart_Data.xlsx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        [HttpGet("SoldProductsByMonth")]
        public IActionResult GetSoldProductsByMonth(string sortBy)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;

                var soldProductsQuery = (from products in _context.Products
                                         join details in _context.ProductDetails on products.Id equals details.Id_Product
                                         join billdetails in _context.BillDetails on details.Id equals billdetails.ProductDetailID
                                         join bills in _context.Bills on billdetails.BIllId equals bills.id
                                         join images in _context.Images on details.Id equals images.IdProductdetail
                                         where bills.Status == 4 && bills.PayDate.HasValue && bills.PayDate.Value >= currentDate.AddDays(-30)
                                         group new { products, details, billdetails, bills, images } by new { products.Name } into grouped
                                         select new
                                         {
                                             ProductName = grouped.Key.Name,
                                             QuantitySold = grouped.Sum(x => x.billdetails.Amount),
                                             TotalEarnings = grouped.Sum(x => x.billdetails.Price),
                                             ProductImage = grouped.FirstOrDefault().images.Name,
                                         });

                // Sắp xếp theo số lượng bán hoặc giá tiền
                switch (sortBy)
                {
                    case "quantity":
                        soldProductsQuery = soldProductsQuery.OrderByDescending(x => x.QuantitySold);
                        break;
                    case "earnings":
                        soldProductsQuery = soldProductsQuery.OrderByDescending(x => x.TotalEarnings);
                        break;
                    default:
                        // Mặc định sắp xếp theo số lượng bán nếu không có hoặc không hợp lệ sortBy
                        soldProductsQuery = soldProductsQuery.OrderByDescending(x => x.QuantitySold);
                        break;
                }

                var soldProducts = soldProductsQuery.Take(5).ToList();

                var responseData = soldProducts.Select(item => new
                {
                    ProductName = item.ProductName,
                    QuantitySold = item.QuantitySold,
                    TotalEarnings = item.TotalEarnings,
                    ProductImage = item.ProductImage
                }).ToList();

                var response = new
                {
                    labels = responseData.Select(item => item.ProductName),
                    quantities = responseData.Select(item => item.QuantitySold),
                    earnings = responseData.Select(item => item.TotalEarnings),
                    images = responseData.Select(item => item.ProductImage),
                    label = "Sản Phẩm Đã Bán Tháng Này"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Customers/Count")]
        public IActionResult GetCustomers()
        {
            try
            {
                var count = _context.Accounts
                    .Where(a => a.Status == 1 && a.Role.name == "Customer")
                    .Count();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Products/Count")]
        public IActionResult GetProducts()
        {
            try
            {
                var totalQuantity = _context.Bills.Where(p => p.Status == 4).SelectMany(bill => bill.BillDetails).Sum(billDetail => billDetail.Amount);
                string formattedQuantity;

                if (totalQuantity > 1000000)
                {
                    formattedQuantity = $"{totalQuantity / 1000000:F2}M";
                }
                else if (totalQuantity > 1000)
                {
                    formattedQuantity = $"{totalQuantity / 1000:F2}K";
                }
                else
                {
                    formattedQuantity = totalQuantity.ToString();
                }

                return Ok(new { totalQuantity = formattedQuantity });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("Revenues/Count")]
        public IActionResult GetRevenues()
        {
            try
            {
                var totalRevenue = _context.Bills.Where(p => p.Status == 4 && p.PayDate != null).Sum(p => p.TotalMoney);
                string formatted;
                if (totalRevenue > 1000000000)
                {
                    formatted = $"{totalRevenue / 1000000000:F2}Tỷ";
                }
                else if (totalRevenue > 1000000)
                {
                    formatted = $"{totalRevenue / 1000000:F2}Tr";
                }
                else if (totalRevenue > 1000)
                {
                    formatted = $"{totalRevenue / 1000:F2}K";
                }
                else
                {
                    formatted = totalRevenue.ToString();
                }
                return Ok(new { totalRevenue = formatted });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("CountBills/Count")]
        public IActionResult CountBill()
        {
            try
            {
                var countBill = _context.Bills
                    .Where(a => a.Status == 4 && a.PayDate != null)
                    .Count();
                string formattedCount;

                if (countBill > 1000000)
                {
                    formattedCount = $"{countBill / 1000000:F2}M";
                }
                else if (countBill > 1000)
                {
                    formattedCount = $"{countBill / 1000:F2}K";
                }
                else
                {
                    formattedCount = countBill.ToString();
                }
                return Ok(new { countBill = formattedCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}