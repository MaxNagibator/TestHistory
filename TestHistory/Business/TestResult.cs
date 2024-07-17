using Microsoft.AspNetCore.Http;

namespace TestHistory.Business
{
    public class TestResult
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string PipeId => GetProperty(Globals.PipeIdParamName);
        public string JobId => GetProperty(Globals.JobIdParamName);
        public string JobName => GetProperty(Globals.JobNameParamName);
        public string Branch => GetProperty(Globals.BranchParamName);
        public string CommitSha => GetProperty(Globals.CommitShaParamName);
        public string CommitTitle => GetProperty(Globals.CommitTitleParamName);

        public TestRunTrx RunResult { get; set; }
        public string DateDir { get; set; }

        public string StorePath => Path.Combine(Globals.Settings.ResultsPath, DateDir, Id.ToString());
        public string RunDeploymentRoot { get; set; }
        public Counters TestCounters { get; set; }
        public Times Times { get; internal set; }

        public string GetProperty(string key)
        {
            return Properties.ContainsKey(key) ? Properties[key] : null;
        }
    }
}
