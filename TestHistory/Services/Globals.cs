
namespace TestHistory.Services
{
    public static class Globals
    {
        public static Settings Settings { get; set; }

        /// <summary>
        /// Чтоб не начать обрабатывать неполноценные данные (например недоконца разархивированную папку).
        /// </summary>
        public static string CompliteFileName = "prepared";
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
        }
    }
}
