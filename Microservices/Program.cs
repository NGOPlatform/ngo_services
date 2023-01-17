namespace Microservices
{
    internal class Program
    {
        // VERY IMPORTANT (SET THE NAME OF THE LOCAL REPO)
        public static readonly string REPO_NAME = "ngo_services";

        public static string FULL_PATH; // this will be initialized based on REPO_NAME

        string amogus = "ඞ";

        static async Task Main(string[] args)
        {
            Initialize();
            NGOEmis.Initialize();

            await NGOFetch.DownloadData();

            string pre = FULL_PATH + @"\list";
            string postE = ".xlsx";
            string postC = ".csv";

            NGOParse.SaveAsCsv(pre + "1" + postE, pre + "1" + postC);
            NGOParse.SaveAsCsv(pre + "2" + postE, pre + "2" + postC);
            NGOParse.SaveAsCsv(pre + "3" + postE, pre + "3" + postC);
            NGOParse.SaveAsCsv(pre + "4" + postE, pre + "4" + postC);

            NGOSeed.Connect();
            NGOSeed.AddCSVToDatabase(pre + "1" + postC);
            NGOSeed.AddCSVToDatabase(pre + "2" + postC);
            NGOSeed.AddCSVToDatabase(pre + "3" + postC);
            NGOSeed.AddCSVToDatabase(pre + "4" + postC);

            NGOSeed.CheckMatches();
            NGOSeed.Disconnect();
        }

        static void Initialize()
        {
            DirectoryInfo di = Directory.GetParent(Directory.GetCurrentDirectory());
            while (di.Name != REPO_NAME)
                di =  Directory.GetParent(di.FullName);

            FULL_PATH =  di.FullName;
        }
    }
}