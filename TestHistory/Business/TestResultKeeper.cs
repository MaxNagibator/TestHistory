using System.Collections.Concurrent;

namespace TestHistory.Business
{
    public class TestResultKeeper
    {
        public ConcurrentDictionary<Guid, TestResult> Results { get; set; } = new ConcurrentDictionary<Guid, TestResult>();

        public TestResultKeeper() 
        { 
        }

        public bool Add(TestResult result)
        {
            return Results.TryAdd(result.Id, result);
        }
    }
}
