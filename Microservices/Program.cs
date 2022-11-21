namespace Microservices
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // testare parser
            string pre = "C:\\Users\\Silviu\\source\\repos\\ngo_services\\list";
            string postE = ".xlsx";
            string postC = ".csv";
            
            NGOParse.SaveAsCsv(pre + "1" + postE, pre + "1" + postC);
            NGOParse.SaveAsCsv(pre + "2" + postE, pre + "2" + postC);
            NGOParse.SaveAsCsv(pre + "3" + postE, pre + "3" + postC);
            NGOParse.SaveAsCsv(pre + "4" + postE, pre + "4" + postC);
            NGOParse.SaveAsCsv(pre + "5" + postE, pre + "5" + postC);
        }
    }
}