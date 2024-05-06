namespace TestHistory.Business
{
    public class TestResult
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string PipeId => GetProperty("pipe");
        public string JobId => GetProperty("jobid");
        public string Branch => GetProperty("branch");
        public TestRunTrx RunResult { get; set; }

        public string GetProperty(string key)
        {
            return Properties.ContainsKey(key) ? Properties[key] : null;
        }
    }
}
