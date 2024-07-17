using TestHistory.Models;

namespace TestHistory.Business
{
    public class HistoryRun
    {
        public TestOutcome Outcome { get; set; }
        public TestResult TestResult { get; set; }
    }
}
