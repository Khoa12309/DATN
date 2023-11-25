using APPDATA.DB;
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
                    .Where(b => b.PayDate.HasValue && b.PayDate.Value.Year == year)
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

                // Lấy dữ liệu từ database cho doanh thu trong 1 tuần qua
                var data = _context.Bills
                    .Where(b => b.PayDate.HasValue && b.PayDate.Value >= currentDate.AddDays(-7))
                    .GroupBy(b => b.PayDate.Value.Date)  // Nhóm theo ngày
                    .OrderBy(group => group.Key)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Money = group.Sum(b => b.TotalMoney)
                    })
                    .ToList();

                // Chuyển đổi dữ liệu thành định dạng phù hợp cho biểu đồ
                var chartData = new
                {
                    labels = data.Select(item => item.Date.ToShortDateString()), // Format ngày thành chuỗi
                    values = data.Select(item => item.Money),
                    label = "Doanh Thu Tuần Qua"
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}