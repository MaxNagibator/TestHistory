namespace TestHistory.Business
{
    public class TestResult
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string PipeId => Properties.ContainsKey("pipe") ? Properties["pipe"] : "-";
        public string JobId => Properties.ContainsKey("job") ? Properties["job"] : "-";
        public TestRunTrx RunResult { get; set; }
    }
}
