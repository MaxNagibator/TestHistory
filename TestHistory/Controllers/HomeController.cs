using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestHistory.Business;
using TestHistory.Models;
using TestHistory.Services;

namespace TestHistory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TestResultKeeper _keeper;
        private readonly HttpClient client;

        public HomeController(ILogger<HomeController> logger, TestResultKeeper keeper)
        {
            _logger = logger;
            _keeper = keeper;

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "TestHistoryApp");
            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", Globals.Settings.GitlabAccessToken);
        }

        public IActionResult Index()
        {
            // https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25007/
            // https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25007/test_report
            var view = _keeper.Results
                .GroupBy(x => x.Value.PipeId)
                .OrderByDescending(x => x.Key)
                .Select(x => new RunDetails
                {
                    Title = x.First().Value.RunResult.Name,
                    Branch = x.First().Value.Branch,
                    //Title = x.First().Value.Properties["commitmessage"],
                    PipeId = x.Key
                })
                .ToArray();
            return View(view);
        }

        [Route("Home/Pipe/{pipeid}")]
        public IActionResult Details(string pipeId)
        {

            // ������� ����� ������ ������ � ����� �������� ���������� :) 
            //        C: \Users\max > curl--header "PRIVATE-TOKEN: {Globals.Settings.GitlabAccessToken}" "https://gitlab.agroassist.ru/api/v4/projects/172/pipelines/25180"
            //{ "id":25180,"iid":101,"project_id":172,"sha":"14a13c41dba1a1ac459c979ff5692e3996a004b5","ref":"bobina","status":"failed","source":"push","created_at":"2024-04-19T06:13:15.317+03:00","updated_at":"2024-04-19T06:18:18.133+03:00","web_url":"https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25180","before_sha":"a914a03f359680ca081155547186879f439ea870","tag":false,"yaml_errors":null,"user":{ "id":105,"username":"m.gricina","name":"������� ������","state":"active","locked":false,"avatar_url":"https://gitlab.agroassist.ru/uploads/-/system/user/avatar/105/avatar.png","web_url":"https://gitlab.agroassist.ru/m.gricina"},"started_at":"2024-04-19T06:13:18.842+03:00","finished_at":"2024-04-19T06:18:18.117+03:00","committed_at":null,"duration":294,"queued_duration":3,"coverage":null,"detailed_status":{ "icon":"status_failed","text":"Failed","label":"failed","group":"failed","tooltip":"failed","has_details":false,"details_path":"/fullstack/web.agrohistoryv2/-/pipelines/25180","illustration":null,"favicon":"/assets/ci_favicons/favicon_status_failed-41304d7f7e3828808b0c26771f0309e55296819a9beea3ea9fbf6689d9857c12.png"},"name":null}
            var result = _keeper.Results.Where(x => x.Value.PipeId == pipeId).Select(x => x.Value).ToArray();
            return View(result);
        }

        public async Task<IActionResult> GetDataFromGitlabPipe(string pipeId)
        {
            var responseString = await client.GetStringAsync("https://gitlab.agroassist.ru/api/v4/projects/172/pipelines/"+ pipeId);
            var model = JsonConvert.DeserializeObject<GitlabPipeDetails>(responseString);
            return PartialView("GitlabPipe", model);
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
