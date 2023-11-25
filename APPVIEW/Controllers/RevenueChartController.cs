using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
	public class RevenueChartController : Controller
	{
		public IActionResult RevenueChart()
		{
			return View();
		}
	}
}
