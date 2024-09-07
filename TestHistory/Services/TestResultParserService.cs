using System.Xml.Serialization;
using TestHistory.Business;

namespace TestHistory.Services
{
    public class TestResultParserService : IHostedService, IDisposable
    {
        private readonly ILogger<TestResultParserService> _logger;
        private readonly TestResultKeeper _testResultKeeper;
        private bool _isInitialComplete;
        private Timer? _timer = null;
        private bool _isInProcess = false;

        public TestResultParserService(ILogger<TestResultParserService> logger, TestResultKeeper testResultKeeper)
        {
            _logger = logger;
            _testResultKeeper = testResultKeeper;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TestResultParserService running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private void DoWork(object? state)
        {
            if (_isInProcess)
            {
                return;
            }
            _isInProcess = true;

            if (!_isInitialComplete)
            {
                _logger.LogInformation("execute ProcessResults");
                ProcessResults();

                _isInitialComplete = true;
            }
            else
            {
                _logger.LogInformation("execute ProcessPrepared");
                ProcessPrepared();

            }

            _isInProcess = false;
        }

        private void ProcessResults()
        {
            var startDate = DateTime.Now;
            var dateDirs = Directory.GetDirectories(Globals.Settings.ResultsPath);
            var processDirs = new List<DirProps>();
            foreach (var dateDir in dateDirs.OrderByDescending(x => x))
            {
                var dt = Path.GetFileName(dateDir);
                var dirs = Directory.GetDirectories(dateDir);
                foreach (var dir in dirs)
                {
                    processDirs.Add(new DirProps { Path = dir, DateDir = dt });
                }
            }
            Parallel.ForEach(processDirs, new ParallelOptions { MaxDegreeOfParallelism = 4 }, dir =>
            {
                var folderName = Path.GetFileName(dir.Path);
                _logger.LogInformation("execute " + dir.DateDir + " " + folderName);
                var result = TestResultParser.ProcessTestDir(dir.Path, (ex, x) => _logger.LogError(ex, x)); // оптимизировать тут
                if (result != null)
                {
                    result.DateDir = dir.DateDir;
                    var success = _testResultKeeper.AddTestResult(result); // оптимизировать тут
                    if (!success)
                    {
                        _logger.LogWarning(result.Id + " exists");
                    }
                }
            });
            var totalTime = (DateTime.Now - startDate).TotalSeconds;
            _logger.LogInformation("load time " + totalTime + "sec");
        }

        private class DirProps
        {
            public string Path { get; set; }
            public string DateDir { get; set; }
        }

        private void ProcessPrepared()
        {
            var dirs = Directory.GetDirectories(Globals.Settings.PreparedPath);
            foreach (var dir in dirs)
            {
                var result = TestResultParser.ProcessTestDir(dir, (ex, x) => _logger.LogError(ex, x));
                if (result != null)
                {
                    //2024-04-16T14:43:08.2032627+07:00
                    var date = DateTime.Parse(result.RunResult.Times.Start);
                    var dt = date.ToString("yyyy.MM.dd");
                    result.DateDir = dt;
                    var success = _testResultKeeper.AddTestResult(result);
                    if (!success)
                    {
                        // такой уже есть, выкидываем в помойку
                        Directory.Delete(dir, true);
                    }
                    else
                    {
                        var target = Path.Combine(Globals.Settings.ResultsPath, dt);
                        if (!Directory.Exists(target))
                        {
                            Directory.CreateDirectory(target);
                        }
                        Directory.Move(dir, Path.Combine(target, result.Id.ToString()));
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TestResultParserService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
