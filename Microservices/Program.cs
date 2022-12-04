namespace Microservices
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // await NGOFetch.DownloadData();

            string pre = @"C:\Users\Silviu\source\repos\ngo_services\list";
            //string postE = ".xlsx";
            string postC = ".csv";

            //NGOParse.SaveAsCsv(pre + "1" + postE, pre + "1" + postC);
            //NGOParse.SaveAsCsv(pre + "2" + postE, pre + "2" + postC);
            //NGOParse.SaveAsCsv(pre + "3" + postE, pre + "3" + postC);
            //NGOParse.SaveAsCsv(pre + "4" + postE, pre + "4" + postC);

            // testare populare date
            NGOSeed.Connect();
            //NGOSeed.AddCSVToDatabase(pre + "1" + postC);
            //NGOSeed.AddCSVToDatabase(pre + "2" + postC);
            //NGOSeed.AddCSVToDatabase(pre + "3" + postC);
            NGOSeed.AddCSVToDatabase(pre + "4" + postC);
            NGOSeed.Disconnect();
        }
    }
}