using Microsoft.AspNetCore.Mvc;
using TestHistory.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultsController : ControllerBase
    {
        /// <summary>
        /// Загрузить архив с результатами тестов.
        /// </summary>
        /// <param name="properties">в виде строки key1=value1;key2=value2</param>
        /// <remarks>
        /// Загрузка через пошик
        ///$boundary = [System.Guid]::NewGuid().ToString()
        ///$FilePath = "E:\geomir\repo\web.agrohistoryv2_6\TestResults\max_BIGPOTATO_2024-04-16_14_43_13.zip"
        ///$TheFile = [System.IO.File]::ReadAllBytes($FilePath)
        ///$TheFileContent = [System.Text.Encoding]::GetEncoding('iso-8859-1').GetString($TheFile)
        ///$LF = "`r`n"
        ///$bodyLines = (
        ///    "--$boundary",
        ///    "Content-Disposition: form-data; name=`"Description`"$LF",
        ///    "This is a file I'm uploading",
        ///    "--$boundary",
        ///    "Content-Disposition: form-data; name=`"TheFile`"; filename=`"file.json`"",
        ///    "Content-Type: application/json$LF",
        ///    $TheFileContent,
        ///    "--$boundary--$LF"
        ///) -join $LF
        ///Invoke-RestMethod 'http://localhost:5047/api/TestResults/CreateFromZip?branch=dev-1&job=2&jobname=apitests&pipe=3'' -Method POST -ContentType "multipart/form-data; boundary=`"$boundary`"" -Body $bodyLines
        /// </remarks>
        /// <returns></returns>
        [HttpPost("CreateFromZip")]
        public int CreateFromZip()
        {
            var properties = string.Join(";", Request.Query.Keys.Select(key => key.ToLower() + "=" + Request.Query[key].ToString().ToLower()));
            var a = 1;
            var file2 = Request.Form.Files[0];
            var fileStream = file2.OpenReadStream();

            var zipName = Guid.NewGuid() + ".zip";
            var zipPath = Path.Combine(Globals.Settings.UploadPath, zipName);
            using (var fileStream3 = System.IO.File.Create(zipPath))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(fileStream3);
            }
            fileStream.Close();

            var resultId = Guid.NewGuid();
            var extractPath = Path.Combine(Globals.Settings.PreparedPath, resultId.ToString()); //date, resultId.ToString());
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            System.IO.File.WriteAllText(Path.Combine(extractPath, "prepared"), properties);
            System.IO.File.Delete(zipPath);
            return 1;
        }
    }
}
