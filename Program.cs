using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CzechJudiciaryDecisionsTools.Common.Interfaces;
using CzechJudiciaryDecisionsTools.Common.Services;
using CzechJudiciaryDecisionsTools.Courts.SupremeCourt;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;


namespace CzechJudiciaryDecisionsTools
{
    class Program
    {
        private static ServiceProvider ServiceProvider;

        static async Task Main(string[] args)
        {
            ServiceProvider = ConfigureServices(new ServiceCollection()).BuildServiceProvider();

            await DownloadDecisionsAsync(new DateTime(2020, 11, 1), new DateTime(2024, 1, 20));
            await DownloadDecisionsAsync(new DateTime(2000, 1, 1), new DateTime(2013, 12, 31));
            await IndexSupremeCourtDecisionsAsync();
        }

        private async static Task DownloadDecisionsAsync(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Downloader is on.");
            List<DateTime> unsuccessfullyAttemptedDates = await DownloadSupremeCourtDecisionsAsync(startDate, endDate);
            await IndexSupremeCourtDecisionsAsync();
            foreach (var date in unsuccessfullyAttemptedDates)
            {
                Console.WriteLine($"Retrying download date {date.ToShortDateString()}");
                List<DateTime> unsuccessfullyAttemptedDates2 = await DownloadSupremeCourtDecisionsAsync(date, date);
                if (unsuccessfullyAttemptedDates2 != null && unsuccessfullyAttemptedDates2.Count > 0)
                {
                    unsuccessfullyAttemptedDates.Add(date);
                }
            }
            Console.WriteLine("All decisions downloaded.");
        }

        private static async Task<List<DateTime>> DownloadSupremeCourtDecisionsAsync(DateTime startDate, DateTime endDate)
        {
            List<DateTime> unsuccessfullyAttemptedDates = [];
            var supremeCourtDownloader = ServiceProvider.GetService<ICourtDecisionDownloader>();

            if (supremeCourtDownloader != null)
            {
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    try
                    {
                        Console.WriteLine("Downloading...");
                        (bool Success, string Message) = await supremeCourtDownloader.DownloadDecisionByDateAsync(date);
                        Console.WriteLine(Message);
                        if (!Success)
                            unsuccessfullyAttemptedDates.Add(date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred on {date.ToShortDateString()}: {ex.Message}");
                        unsuccessfullyAttemptedDates.Add(date);
                    }
                }
            }
            else
            {
                Console.WriteLine("Error getting Supreme Court Downloader service.");
            }

            return unsuccessfullyAttemptedDates;
        }
        private static async Task<int> IndexSupremeCourtDecisionsAsync()
        {
            var supremeCourtIndexer = ServiceProvider.GetService<ICourtIndexer>();
            int indexedCount = 0;

            if (supremeCourtIndexer != null)
            {
                Console.WriteLine($"Indexing Supreme Court decisions..");
                var decisions = await supremeCourtIndexer.IndexDecisionsAsync();
                Console.WriteLine($"All {decisions.Count} Supreme Court decisions indexed.");
            }
            else
            {
                Console.WriteLine("Error getting Supreme Court Indexer service.");
            }

            return indexedCount;
        }


        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IDownloader, HttpDownloader>();
            services.AddSingleton<ICourtDecisionDownloader, SupremeCourtDownloader>();
            services.AddSingleton<ICourtIndexer, SupremeCourtIndexer>();// For Supreme Court
            // Add other courts here as needed

            // Configure logging
            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddProvider(new CustomFileLoggerProvider(new StreamWriter("Logs/logs.txt")));
            });
            return services;
        }
    }
}
