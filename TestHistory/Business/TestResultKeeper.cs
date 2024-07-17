using System.Collections.Concurrent;
using Microsoft.AspNetCore.Rewrite;

namespace TestHistory.Business
{
    public class TestResultKeeper
    {
        private ConcurrentDictionary<Guid, TestResult> Results { get; set; } = new ConcurrentDictionary<Guid, TestResult>();
        private Dictionary<string, TestResultGroup> ResultGroups { get; set; } = new Dictionary<string, TestResultGroup>();
        private Dictionary<string, List<HistoryRun>> HistoryContainers { get; set; } = new Dictionary<string, List<HistoryRun>>();


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

                    result.RunDeploymentRoot = result.RunResult.TestSettings.Deployment.RunDeploymentRoot;
                    result.TestCounters = result.RunResult.ResultSummary.Counters;
                    result.Times = result.RunResult.Times;
                    var runResults = result.RunResult?.Results?.UnitTestResults;
                    if (runResults != null)
                    {
                        foreach (var runResult in runResults)
                        {
                            var runOutcome = Globals.TestOutcomes.ContainsKey(runResult.Outcome) ? Globals.TestOutcomes[runResult.Outcome] : TestOutcome.Unknown;
                            var test = result.RunResult.TestDefinitions.UnitTest.First(x => x.Id == runResult.TestId);
                            var name = test.TestMethod.ClassName + "." + test.TestMethod.Name;
                            if (!HistoryContainers.ContainsKey(name))
                            {
                                HistoryContainers.Add(name, new List<HistoryRun>());
                            }
                            HistoryContainers[name].Add(new HistoryRun { Outcome = runOutcome, TestResult = result });
                        }
                    }
                    // заберём нужную инфу и освободим оперативку, будем подсасывать с фаила на просмотр уже
                    result.RunResult = null;
                }
            }
            return added;
        }

        public List<HistoryRun> GetHistoryContainer(string testName)
        {
            return HistoryContainers[testName];
        }

        public TestResult GetTestResult(Guid resultId)
        {
            return Results[resultId];
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
