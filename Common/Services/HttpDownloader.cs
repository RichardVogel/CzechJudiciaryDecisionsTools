using CzechJudiciaryDecisionsTools.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Common.Services
{
    public class HttpDownloader : IDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpDownloader> _logger;

        public HttpDownloader(HttpClient httpClient, ILogger<HttpDownloader> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> DownloadAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Starting download from: {url}");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Download successful.");
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error occurred while downloading from {url}.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during download.");
                throw;
            }
        }
        public async Task<string> DownloadZipAsync(string url, string outputPath)
        {
            try
            {
                _logger.LogInformation($"Starting ZIP download from: {url}");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Save the response as a ZIP file
                using (var fs = new FileStream(outputPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }

                _logger.LogInformation("ZIP download successful.");
                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading ZIP from {url}.");
                throw;
            }
        }
    }
}
