using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Hosting.Internal;

namespace CRM.Services
{
    public class MyLoggerService : DevExpress.XtraReports.Web.ClientControls.LoggerService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MyLoggerService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public override void Info(string message)
        {
             SaveDataToFileAsync(GenerateUniqueFileName("ReportingLog"),string.Format( "[{0}]: Info: '{1}'.", DateTime.Now, message));
            //System.Diagnostics.Debug.WriteLine("[{0}]: Info: '{1}'.", DateTime.Now, message);
        }
        public override void Error(Exception exception, string message)
        {
            SaveDataToFileAsync(GenerateUniqueFileName("ReportingLog"), string.Format("[{0}]: Exception occured. Message: '{1}'. Exception Details:\r\n{2}",
                DateTime.Now, message, exception));

            //System.Diagnostics.Debug.WriteLine("[{0}]: Exception occured. Message: '{1}'. Exception Details:\r\n{2}",
            //    DateTime.Now, message, exception);
        }
        private  void SaveDataToFileAsync(string fileName, string content)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, fileName);

             using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                 writer.WriteLineAsync(content);
            }
        }
        private string GenerateUniqueFileName(string baseName)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string randomId = Guid.NewGuid().ToString("N").Substring(0, 8);
            string uniqueFileName = $"{baseName}_{timestamp}_{randomId}.txt";
            return uniqueFileName;
        }
    }
}
