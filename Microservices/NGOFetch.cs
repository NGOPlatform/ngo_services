using System.Net;
using System.Text.RegularExpressions;

namespace Microservices
{
    internal class NGOFetch
    {
        static readonly HttpClient client = new();
        static readonly WebClient webClient = new();

        static async Task Main()
        {
            string responseText = "";

            Console.WriteLine("Fetching download links from site...");
            // se incearca extragerea codului html de pe site-ul cu fisierele xlsl
            try
            {
                using HttpResponseMessage response = await client.GetAsync("https://www.just.ro/registrul-national-ong/");
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nProgram threw following exception:");
                Console.WriteLine(e.Message);
            }

            // se extrag, din site, link-urile pentru fisierele xlsl
            List<string> fileURL = new();
            Match match = Regex.Match(responseText, "https:\\/\\/www\\.just\\.ro\\/wp-content\\/uploads\\/[0-9]{4}\\/[0-9]{2}\\/[0-9A-Za-z._]*\\.xlsx");
            
            while (match.Success)
            {
                fileURL.Add(match.Value);
                match = match.NextMatch();
            }

            // se elimina lista dovezilor, deoarece nu este un fisier relevant
            fileURL.RemoveAt(fileURL.Count - 1);

            // se descarca fisierele xlsl
            Console.WriteLine("Downloading files... (This may take a minute or two)");
            for (int i = 0; i < fileURL.Count; i++)
                webClient.DownloadFile(fileURL[i], "C:\\Users\\Silviu\\source\\repos\\ngo_services\\list" + (i+1).ToString() + ".xlsx");

            Console.WriteLine("\n\nDownload Complete!");
        }
    }
}
