using TestHistory.Business;

namespace TestHistory.Models
{
    public class HistoryModel
    {
        public string TestName { get; set; }

        public List<HistoryRun> HistoryRuns { get; set; }
    }
}
