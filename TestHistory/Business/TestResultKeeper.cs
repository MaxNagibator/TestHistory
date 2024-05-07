using System.Collections.Concurrent;

namespace TestHistory.Business
{
    public class TestResultKeeper
    {
        private ConcurrentDictionary<Guid, TestResult> Results { get; set; } = new ConcurrentDictionary<Guid, TestResult>();
        private Dictionary<string, TestResultGroup> ResultGroups { get; set; } = new Dictionary<string, TestResultGroup>();

        /// <summary>
        /// Нагрузка не конская, подождут.
        /// </summary>
        private object _lock = new object();
        private GitlabService _gitlabService;

        public TestResultKeeper(GitlabService gitlabService)
        {
            _gitlabService = gitlabService;
        }

        public bool AddTestResult(TestResult result)
        {
            var pipeId = result.PipeId;
            var added = Results.TryAdd(result.Id, result);
            if (added)
            {
                lock (_lock)
                {
                    if (!ResultGroups.ContainsKey(pipeId))
                    {
                        var group = new TestResultGroup { TestResults = new List<TestResult>() };
                        group.Branch = result.Branch;
                        group.PipeId = pipeId;
                        group.CommitSha = result.CommitSha;
                        group.CommitTitle = result.CommitTitle;
                        group.GitlabData = _gitlabService.GetGitlabData(pipeId);
                        ResultGroups.Add(pipeId, group);
                    }
                    ResultGroups[pipeId].TestResults.Add(result);
                }
            }
            return added;
        }

        public TestResultGroup[] GetTestResultGroups()
        {
            return ResultGroups.Values.ToArray();
        }

        public TestResultGroup GetTestResultGroup(string pipeId)
        {
            return ResultGroups[pipeId];
        }
    }
}
