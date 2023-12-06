using APPDATA.DB;
using APPDATA.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ShoppingDB _context;

        public ChartController()
        {
            _context = new ShoppingDB(new DbContextOptions<ShoppingDB>());
        }

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

        [HttpGet("SoldProductsByMonth")]
        public IActionResult GetSoldProductsByMonth()
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;

                var soldProducts = (from products in _context.Products
                                   join details in _context.ProductDetails on products.Id equals details.Id_Product
                                   join billdetails in _context.BillDetails on details.Id equals billdetails.ProductDetailID
                                   join bills in _context.Bills on billdetails.BIllId equals bills.id
                                   where bills.Status == 4 && bills.PayDate.HasValue && bills.PayDate.Value >= currentDate.AddDays(-30)
                                    group new { products, details, billdetails, bills } by new { products.Name } into grouped
                                   select new
                                   {
                                       ProductName = grouped.Key.Name,
                                       QuantitySold = grouped.Sum(x => x.billdetails.Amount),
                                       TotalEarnings = grouped.Sum(x => x.billdetails.Price)
                                   }).OrderByDescending(x => x.QuantitySold).Take(5).ToList();
                // Chuyển đổi dữ liệu thành định dạng phù hợp cho JSON
                var responseData = new
                {
                    labels = soldProducts.Select(item => item.ProductName),
                    quantities = soldProducts.Select(item => item.QuantitySold),
                    earnings = soldProducts.Select(item => item.TotalEarnings),
                    label = "Sản Phẩm Đã Bán Tháng Này"
                };

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}