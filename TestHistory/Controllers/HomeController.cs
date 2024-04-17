using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestHistory.Business;
using TestHistory.Models;

namespace TestHistory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TestResultKeeper _keeper;

        public HomeController(ILogger<HomeController> logger, TestResultKeeper keeper)
        {
            _logger = logger;
            _keeper = keeper;
        }

        public IActionResult Index()
        {
            // https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25007/
            // https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25007/test_report
            var view = _keeper.Results.GroupBy(x => x.Value.PipeId)
                .Select(x => new RunDetails
                {
                    Title = x.First().Value.RunResult.Name,
                    //Title = x.First().Value.Properties["commitmessage"],
                    PipeId = x.Key
                })
                .ToArray();
            return View(view);
        }

        [Route("Home/Pipe/{pipeid}")]
        public IActionResult Details(string pipeId)
        {
            var result = _keeper.Results.Where(x => x.Value.PipeId == pipeId).Select(x => x.Value).ToArray();
            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
