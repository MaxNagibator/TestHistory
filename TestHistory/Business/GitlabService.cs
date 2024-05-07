using Newtonsoft.Json;
using TestHistory.Models;

namespace TestHistory.Business
{
    public class GitlabService
    {
        private readonly ILogger<GitlabService> _logger;
        private readonly HttpClient _client;

        public GitlabService(ILogger<GitlabService> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "TestHistoryApp");
            _client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", Globals.Settings.GitlabAccessToken);
        }

        public GitlabPipeDetails GetGitlabData(string pipeId)
        {
            var pipeIdCache = Path.Combine(Globals.Settings.GitlabCachePath, pipeId + ".details.txt");
            string responseString;
            if (File.Exists(pipeIdCache))
            {
                responseString = File.ReadAllText(pipeIdCache);
            }
            else
            {
                try
                {
                    var url = "https://gitlab.agroassist.ru/api/v4/projects/172/pipelines/" + pipeId;
                    var task = Task.Run(() => _client.GetStringAsync(url));
                    task.Wait();
                    responseString = task.Result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ошибка получения данных");
                    return null;
                }
                try
                {
                    File.WriteAllText(pipeIdCache, responseString);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ошибка записи в кэш");
                    return null;
                }
            }
            try
            {
                var model = JsonConvert.DeserializeObject<GitlabPipeDetails>(responseString);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ошибка парсинга данных");
                return null;
            }
        }
    }
}
