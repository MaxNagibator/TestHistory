using TestHistory.Models;

namespace TestHistory.Business
{
    public class TestResultGroup
    {
        public string PipeId { get; set; }
        public string Branch { get; set; }
        public List<TestResult> TestResults { get; set; }
        public GitlabPipeDetails GitlabData { get; set; }
        public string CommitTitle { get; set; }
        public string CommitSha { get; set; }
    }
}
