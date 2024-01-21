using System;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CzechJudiciaryDecisionsTools.Common.Interfaces;
using CzechJudiciaryDecisionsTools.Common.Services;

namespace CzechJudiciaryDecisionsTools.Courts.SupremeCourt
{
    public class SupremeCourtDownloader : ICourtDecisionDownloader
    {
        private readonly IDownloader _downloader;
        private const string BaseUrl = "https://www.nsoud.cz/Judikatura/judikatura_ns.nsf/zip?openAgent&query=";
        private string DecisionFolder = Path.Combine(AppContext.BaseDirectory, @"SupremeCourt/Decisions");
        private string TempFolder = Path.Combine(AppContext.BaseDirectory, @"Temp");

        public SupremeCourtDownloader(IDownloader downloader)
        {
            _downloader = downloader;
            Directory.CreateDirectory(DecisionFolder);
            Directory.CreateDirectory(TempFolder);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<(bool Success, string Message)> DownloadDecisionByDateAsync(DateTime date)
        {
            string formattedDate = date.ToString("dd/MM/yyyy");
            string query = $"[datum_predani_na_web]>={formattedDate} AND [datum_predani_na_web]<={formattedDate} AND [SoudCreate]=\"Nejvyšší soud\"&SearchOrder=1&SearchMax=0&start=0&count=15&pohled=";
            string url = $"{BaseUrl}{query}";
            string zipTempFilePath = $"{TempFolder}/SC_{date:yyyy-mm-dd}.zip";

            try
            {
                zipTempFilePath = await _downloader.DownloadZipAsync(url, zipTempFilePath);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred for {formattedDate} during downloading of a ZIP file: {ex.Message}.");
            }
            if (!File.Exists(zipTempFilePath))
                return (false, $"An error occurred for {formattedDate}, ZIP file not downloaded.");

            long fileSize = new FileInfo(zipTempFilePath).Length;
            if (fileSize == 103)
            {
                return (true, $"No Supreme Court decisions for {formattedDate}.");
            }
            else if (fileSize == 143)
            {
                return (false, $"ERROR: Database of Supreme Court decisions for {formattedDate} stated: 'Agent done'.");
            }
            try
            {
                int extractedFilesCount = await ExtractConvertAndSaveZipAsync(zipTempFilePath, DecisionFolder);
                return (true, $"All {extractedFilesCount} Supreme Court decisions for {formattedDate} downloaded and extracted.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred for {formattedDate} during extraction of a ZIP file: {ex.Message}.");
            }
            
        }

        private async Task<int> ExtractConvertAndSaveZipAsync(string zipPath, string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string extractedFilePath = $"{outputFolder}/{entry.FullName}";
                    entry.ExtractToFile(extractedFilePath, overwrite: true);
                    await FileConverter.ConvertFileEncodingToUtf8Async(extractedFilePath, Encoding.GetEncoding(1250));
                }
                return archive?.Entries?.Count ?? 0;
            }
        }
    }
}
