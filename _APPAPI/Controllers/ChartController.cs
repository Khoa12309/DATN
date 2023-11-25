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
        //private readonly CRUDapi<Bill> _crud;

        //private readonly IConfiguration _Configuration;
        public ChartController()
        {
            //_crud = new CRUDapi<Bill>(_context, _context.Bills);
            //_Configuration = configuration;
            _context = new ShoppingDB(new DbContextOptions<ShoppingDB>());
        }
        [HttpGet("{year}")]
        public IActionResult StatisticsByYear(int year)
        {
            try
            {
                var data = _context.Bills
                    .Where(b => b.PayDate.HasValue && b.PayDate.Value.Year == year)
                    .OrderBy(b => b.PayDate)
                    .Select(b => new { Month = b.PayDate.Value.Month, Money = b.TotalMoney })
                    .ToList();

                // Chuyển đổi dữ liệu thành định dạng phù hợp cho biểu đồ
                var chartData = new
                {
                    labels = data.Select(bill => bill.Month),
                    values = data.Select(bill => bill.Money),
                    label = "Doanh Thu"
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