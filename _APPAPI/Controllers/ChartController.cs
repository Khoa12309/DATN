using APPDATA.DB;
using APPDATA.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

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
                        soldProductsQuery = soldProductsQuery.OrderByDescending(x => x.TotalEarnings);
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


        [HttpGet("TopProduct")]
        public IActionResult TopProduct()
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                DateTime startDate = currentDate.AddDays(-15);

                // Tạo danh sách ngày mong muốn
                List<DateTime> dateRange = Enumerable.Range(0, 15)
                                                     .Select(i => startDate.AddDays(i))
                                                     .ToList();

                var soldProducts = (from products in _context.Products
                                    join details in _context.ProductDetails on products.Id equals details.Id_Product
                                    join billdetails in _context.BillDetails on details.Id equals billdetails.ProductDetailID
                                    join bills in _context.Bills on billdetails.BIllId equals bills.id
                                    // Sử dụng danh sách ngày mong muốn
                                    where bills.Status == 4 && bills.PayDate.HasValue && dateRange.Contains(bills.PayDate.Value.Date)
                                    group new { products, details, billdetails, bills } by new { products.Id, products.Name, bills.PayDate.Value.Date } into grouped
                                    select new
                                    {
                                        ProductName = grouped.Key.Name,
                                        Date = grouped.Key.Date,
                                        QuantitySold = grouped.Sum(x => x.billdetails.Amount),
                                    }).OrderBy(x => x.Date).ThenByDescending(x => x.QuantitySold).ToList();

                var responseData = soldProducts.GroupBy(item => item.ProductName).Select(group => new
                {
                    ProductName = group.Key,
                    Dates = dateRange,
                    Quantities = dateRange.Select(date => group.FirstOrDefault(item => item.Date == date)?.QuantitySold ?? 0),
                }).ToList();

                var response = new
                {
                    labels = dateRange,
                    datasets = responseData.Select(item => new
                    {
                        label = item.ProductName,
                        data = item.Quantities,
                    }).Take(5).ToList(),
                };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        //[HttpGet("getDoanhThu7dayOfAProduct")]
        //public IActionResult getDoanhThu7DaysOfAProduct(string productID)
        //{
        //  //  try
        //  //  {
        //     //   var response = new
        //    //    {
        //    //       labels = responseData.Select(item => item.ProductName),
        //     //       quantities = responseData.Select(item => item.QuantitySold),
        //     //       earnings = responseData.Select(item => item.TotalEarnings),
        //     //       label = "Sản Phẩm Đã Bán Tháng Này"
        //   //     };
        //        // viet tiep
        //        // lấy doanh thu 7 ngày gần nhất của 1 sản phẩm
        //        // productID là id của sản phẩm hoặc là name (tuỳ m)
        //        // trả về 1 datasets như test bên viewu
        //        // cần thì viết thêm 1 hàm trả về mảng gồm 5 sản phẩm trong top 5 sau đấy gọi bằng chính hàm này.
        //       // return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}




    }
}