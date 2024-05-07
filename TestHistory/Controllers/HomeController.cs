using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestHistory.Business;
using TestHistory.Models;

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
            var view = _keeper.GetTestResultGroups()
                .OrderByDescending(x => x.PipeId)
                //.Select(x => new RunDetails
                //{
                //    //Title = x.First().Value.RunResult.Name,
                //    Branch = x.Branch,
                //    //Title = x.First().Value.Properties["commitmessage"],
                //    PipeId = x.PipeId
                //})
                .ToArray();
            return View(view);
        }

        [Route("/{pipeid}")]
        [Route("/Pipe/{pipeid}")]
        public async Task<IActionResult> Details(string pipeId)
        {

            // асинком потом запрос кинуть и можно аватарки подгружать :) 
            //        C: \Users\max > curl--header "PRIVATE-TOKEN: {Globals.Settings.GitlabAccessToken}" "https://gitlab.agroassist.ru/api/v4/projects/172/pipelines/25180"
            //{ "id":25180,"iid":101,"project_id":172,"sha":"14a13c41dba1a1ac459c979ff5692e3996a004b5","ref":"bobina","status":"failed","source":"push","created_at":"2024-04-19T06:13:15.317+03:00","updated_at":"2024-04-19T06:18:18.133+03:00","web_url":"https://gitlab.agroassist.ru/fullstack/web.agrohistoryv2/-/pipelines/25180","before_sha":"a914a03f359680ca081155547186879f439ea870","tag":false,"yaml_errors":null,"user":{ "id":105,"username":"m.gricina","name":"Грицина Максим","state":"active","locked":false,"avatar_url":"https://gitlab.agroassist.ru/uploads/-/system/user/avatar/105/avatar.png","web_url":"https://gitlab.agroassist.ru/m.gricina"},"started_at":"2024-04-19T06:13:18.842+03:00","finished_at":"2024-04-19T06:18:18.117+03:00","committed_at":null,"duration":294,"queued_duration":3,"coverage":null,"detailed_status":{ "icon":"status_failed","text":"Failed","label":"failed","group":"failed","tooltip":"failed","has_details":false,"details_path":"/fullstack/web.agrohistoryv2/-/pipelines/25180","illustration":null,"favicon":"/assets/ci_favicons/favicon_status_failed-41304d7f7e3828808b0c26771f0309e55296819a9beea3ea9fbf6689d9857c12.png"},"name":null}
            var result = _keeper.GetTestResultGroup(pipeId);
            //if(result.GitlabData == null)
            //try
            //{
            //    var gitlabData = await GetGitlabData(pipeId);
            //    result.GitlabData = gitlabData;
            //}
            //catch
            //{

            //}
            return View(result);
        }

        //public async Task<IActionResult> GetDataFromGitlabPipe(string pipeId)
        //{
        //    var model = await GetGitlabData(pipeId);
        //    return PartialView("GitlabPipe", model);
        //}

        //private async Task<GitlabPipeDetails> GetGitlabData(string pipeId)
        //{
        //    var pipeIdCache = Path.Combine(Globals.Settings.ResultsPath, pipeId + ".details.txt");
        //    string responseString;
        //    if (System.IO.File.Exists(pipeIdCache))
        //    {
        //        responseString = System.IO.File.ReadAllText(pipeIdCache);
        //    }
        //    else
        //    {
        //        responseString = await client.GetStringAsync("https://gitlab.agroassist.ru/api/v4/projects/172/pipelines/" + pipeId);
        //    }
        //    var model = JsonConvert.DeserializeObject<GitlabPipeDetails>(responseString);
        //    return model;
        //}

        [Route("/Files/{resultId}/{relativeResultsDirectory}/{*path}")]
        public async Task<IActionResult> GetFile(Guid resultId, Guid relativeResultsDirectory, string path)
        {
            var result = _keeper.GetTestResult(resultId);
            var root = result.RunResult.TestSettings.Deployment.RunDeploymentRoot;
            var fullPath = Path.Combine(Globals.Settings.ResultsPath, result.DateDir, resultId.ToString(), root, "in", relativeResultsDirectory.ToString(), path);
            //var path2 = "D:\\tmp\\TestHistory\\Data\\Results\\2024.04.26\\be98fc0f-df01-4ac4-9bb1-cc2a03a60996" +
            //    "\\GitLabRunner_AZUREPIPE-01_2024-04-26_09_21_24\\In\\1cc1dd22-c3e4-4ea8-a1d4-1744552c54d5" +
            //    "\\AZUREPIPE-01\\error_20240426_093303_026__DisableApprovalStatusTest.png";
            //d:\tmp\TestHistory\Data\Results\be98fc0f-df01-4ac4-9bb1-cc2a03a60996\GitLabRunner_AZUREPIPE-01_2024-04-26_09_21_24\in\3957580f-6531-4ee4-9c92-3e87d1755bcb\AZUREPIPE-01\error_20240426_093645_497__AddTechnologyFromFieldInfoTest.png
            var fileName = Path.GetFileName(fullPath);
            string file_type = "image/png";
            return File(System.IO.File.ReadAllBytes(fullPath), "image/png", fileName);

            using (var str = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                return new FileStreamResult(str, file_type)
                {
                    FileDownloadName = fileName
                };
                return File(str, file_type, fileName);
            }

            return PhysicalFile(fullPath, file_type, fileName);
            //http://localhost:5047/Files/be98fc0f-df01-4ac4-9bb1-cc2a03a60996/AZUREPIPE-01/error_20240426_093645_497__AddTechnologyFromFieldInfoTest.png
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
