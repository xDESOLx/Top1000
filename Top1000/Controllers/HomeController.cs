using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Top1000.Models;
using Top1000.Services;

namespace Top1000.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStackExchangeClient _stackExchangeClient;

        public HomeController(ILogger<HomeController> logger, IStackExchangeClient stackExchangeClient)
        {
            _logger = logger;
            _stackExchangeClient = stackExchangeClient;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _stackExchangeClient.GetMostPopularTagsAsync(1000);
            return View(new HomeViewModel()
            {
                Tags = result.ToList(),
                TotalTagUsage = result.Sum(tag => tag.Count)
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
