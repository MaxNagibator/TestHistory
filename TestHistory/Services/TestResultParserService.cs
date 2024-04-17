using System.Xml.Serialization;
using TestHistory.Business;

namespace TestHistory.Services
{
    public class TestResultParserService : IHostedService, IDisposable
    {
        private int executionCount = 0;
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
            _logger.LogInformation("TestResultParserService running.");

            var dateDirs = Directory.GetDirectories(Globals.Settings.ResultsPath);
            foreach (var dateDir in dateDirs.OrderBy(x => x))
            {
                _logger.LogInformation("check " + dateDir);
                var dirs = Directory.GetDirectories(dateDir);
                foreach (var dir in dirs)
                {
                    _logger.LogInformation("check " + dateDir);
                    var result = ProcessTestDir(dir);
                    if (result != null)
                    {
                        var success = _testResultKeeper.Add(result);
                        if (!success)
                        {
                            _logger.LogWarning(result.Id + " exists");
                        }
                    }
                }
            }
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
                var result = ProcessTestDir(dir);
                if (result != null)
                {
                    var success = _testResultKeeper.Add(result);
                    if (!success)
                    {
                        // такой уже есть, выкидываем в помойку
                        Directory.Delete(dir, true);
                    }
                    else
                    {
                        //2024-04-16T14:43:08.2032627+07:00
                        var date = DateTime.Parse(result.RunResult.Times.Start);
                        var dt = date.ToString("yyyy.MM.dd");
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

        private TestResult ProcessTestDir(string dir)
        {
            var complete = Path.Combine(dir, Globals.CompliteFileName);
            if (!File.Exists(complete))
            {
                return null;
            }

            var propsText = File.ReadAllText(complete);
            var trxFiles = Directory.GetFiles(dir, "*.trx");
            if (trxFiles.Count() == 0)
            {
                return null;
            }
            var r = new TestResult();

            XmlSerializer serializer = new XmlSerializer(typeof(TestRunTrx));
            using (Stream reader = new FileStream(trxFiles[0], FileMode.Open))
            {
                var trt = (TestRunTrx)serializer.Deserialize(reader);
                r.RunResult = trt;
                r.Id = Guid.Parse(trt.Id);

                r.Properties = new Dictionary<string, string>();
                if (propsText.Contains("="))
                {
                    var byDelimeter = propsText.Split(';');
                    foreach (var splt in byDelimeter)
                    {
                        var keyValue = splt.Split('=');
                        r.Properties.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            return r;
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
