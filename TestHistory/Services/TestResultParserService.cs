using System.Xml.Serialization;
using TestHistory.Business;

namespace TestHistory.Services
{
    public class TestResultParserService : IHostedService, IDisposable
    {
        private readonly ILogger<TestResultParserService> _logger;
        private readonly TestResultKeeper _testResultKeeper;
        private Timer? _timer = null;
        private bool _isInProcess = false;

        public TestResultParserService(ILogger<TestResultParserService> logger, TestResultKeeper testResultKeeper)
        {
            _logger = logger;
            _testResultKeeper = testResultKeeper;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            var startDate = DateTime.Now;
            _logger.LogInformation("TestResultParserService running.");

            var dateDirs = Directory.GetDirectories(Globals.Settings.ResultsPath);
            foreach (var dateDir in dateDirs.OrderBy(x => x))
            {
                _logger.LogInformation("check " + dateDir);
                var dt = Path.GetFileName(dateDir);
                var dirs = Directory.GetDirectories(dateDir);
                foreach (var dir in dirs)
                {
                    var folderName = Path.GetFileName(dir);
                    _logger.LogInformation("check " + folderName);
                    var result = TestResultParser.ProcessTestDir(dir);
                    if (result != null)
                    {
                        result.DateDir = dt;
                        var success = _testResultKeeper.AddTestResult(result);
                        if (!success)
                        {
                            _logger.LogWarning(result.Id + " exists");
                        }
                    }
                }
            }
            var totalTime = (DateTime.Now - startDate).TotalSeconds;
            _logger.LogInformation("load time " + totalTime + "sec");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private void DoWork(object? state)
        {
            if (_isInProcess)
            {
                return;
            }
            _isInProcess = true;

            var dirs = Directory.GetDirectories(Globals.Settings.PreparedPath);
            foreach (var dir in dirs)
            {
                var result = TestResultParser.ProcessTestDir(dir);
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

            _isInProcess = false;
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
