using Oracle.ManagedDataAccess.Client;

namespace Microservices
{
    internal class NGOSeed
    {
        public static void AddCSVToDatabase(string csvFilePath)
        {
            string conString = "User Id=ADMIN;" +
                               "Password=whatPassword1;" +
                               "Data Source=ez2pdn49ssbxne22_high";

            OracleConfiguration.TnsAdmin = @"C:\Users\Silviu\source\repos\ngo_services\Microservices\Wallet";
            OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

            using OracleConnection con = new OracleConnection(conString);
            using OracleCommand cmd = con.CreateCommand();

            con.Open();
            Console.WriteLine("yay!");
            Console.WriteLine("Versiune: " + con.ServerVersion);
        }
    }
}
