using System.Xml.Serialization;

namespace TestHistory.Business
{
    public static class TestResultParser
    {
        public static TestResult ProcessTestDir(string dir)
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
                        if (splt.Contains("="))
                        {
                            var keyValue = splt.Split('=');
                            r.Properties.Add(keyValue[0], keyValue[1]);
                        }
                    }
                }
            }
            return r;
        }

    }
}
