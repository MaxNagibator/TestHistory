using Microsoft.AspNetCore.Rewrite;

namespace TestHistory.Business
{
    public static class Globals
    {
        public static Settings Settings { get; set; }

        /// <summary>
        /// Чтоб не начать обрабатывать неполноценные данные (например недоконца разархивированную папку).
        /// </summary>
        public static string CompliteFileName = "prepared";

        public static string PipeIdParamName = "pipe";
        public static string JobIdParamName = "jobid";
        public static string JobNameParamName = "jobname";
        public static string BranchParamName = "branch";
        public static string CommitShaParamName = "commitsha";
        public static string CommitTitleParamName = "committitle";
        public static Dictionary<string, TestOutcome> TestOutcomes = new Dictionary<string, TestOutcome>
        {
             { "Passed", TestOutcome.Passed },
             { "Failed", TestOutcome.Failed },
             { "NotExecuted", TestOutcome.NotExecuted },
        };
    }

    public class Settings
    {
        public string DataPath { get; set; }

        public string GitlabAccessToken { get; set; }

        /// <summary>
        /// Сюда заливаем свежие результаты.
        /// </summary>
        public string UploadPath => Path.Combine(DataPath, "Upload");

        /// <summary>
        /// Здесь храним распакованные и готовые к обработке.
        /// </summary>
        public string PreparedPath => Path.Combine(DataPath, "Prepared");

        /// <summary>
        /// Сюда перемещаем обработанные результаты.
        /// </summary>
        public string ResultsPath => Path.Combine(DataPath, "Results");

        /// <summary>
        /// Инфу с гитлаба не будем лишний раз дёргать.
        /// </summary>
        public string GitlabCachePath => Path.Combine(DataPath, "GitlabCache");

        public void Init()
        {
            if (!Directory.Exists(UploadPath))
            {
                Directory.CreateDirectory(UploadPath);
            }
            if (!Directory.Exists(PreparedPath))
            {
                Directory.CreateDirectory(PreparedPath);
            }
            if (!Directory.Exists(ResultsPath))
            {
                Directory.CreateDirectory(ResultsPath);
            }
            if (!Directory.Exists(GitlabCachePath))
            {
                Directory.CreateDirectory(GitlabCachePath);
            }
        }
    }
}
