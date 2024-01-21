namespace CzechJudiciaryDecisionsTools.Common.Services
{
    public interface IDownloader
    {
        public Task<string> DownloadAsync(string url);
        public Task<string> DownloadZipAsync(string url, string outputPath);
    }
}