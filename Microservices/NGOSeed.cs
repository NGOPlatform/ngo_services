using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Microservices
{
    internal static class NGOSeed
    {
        static readonly string conString = "User Id=ADMIN;" +
                                           "Password=whatPassword1;" +
                                           "Data Source=ez2pdn49ssbxne22_high";

        static OracleConnection con;

        public static void Connect()
        {
            OracleConfiguration.TnsAdmin = @"C:\Users\Silviu\Downloads\Wallet";
            OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

            con = new OracleConnection(conString);
            con.Open();
        }

        public static void Disconnect()
        {
            con.Close();
            con.Dispose();
        }

        public static void AddCSVToDatabase(string csvFilePath)
        {
            // verificare conexiune baza date
            if (con == null)
                throw new Exception("Not connected to database!");

            // preluare date din fisier
            using StreamReader file = new(csvFilePath);

            char separator = 'ඞ';
            Dictionary<string, string> dict = new();

            List<string> coloana = file.ReadLine().Split(separator).ToList();
            coloana.RemoveAt(coloana.Count - 1); // ultimul este "", deci este in plus si se va sterge

            List<string> date = file.ReadToEnd().Split(separator).ToList();

            DataTable dt = new();
            dt.Columns.Add("DENUMIRE");
            dt.Columns.Add("NR_INREG");
            dt.Columns.Add("JUDET");
            dt.Columns.Add("LOCALITATE"); 
            dt.Columns.Add("ADRESA");
            dt.Columns.Add("DESCRIERE");

            for (int contor = 0; contor < date.Count; contor++)
            {
                date[contor] = date[contor].Trim();
                dict[coloana[contor % coloana.Count]] = date[contor];

                if ((contor + 1) % coloana.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["DENUMIRE"]  = dict["Denumire"];
                    dr["NR_INREG"]  = dict["Numar inreg Reg National"];
                    dr["JUDET"]     = dict["Judet"];
                    dr["LOCALITATE"]= dict["Localitate"];
                    dr["ADRESA"]    = dict["Adresa"];
                    dr["DESCRIERE"] = dict["Scopul initial"] +
                                      (dict["Modificari ale scopului 1"] == "" ? "" : "\n" + dict["Modificari ale scopului 1"]) +
                                      (dict["Modificari ale scopului 2"] == "" ? "" : "\n" + dict["Modificari ale scopului 2"]) +
                                      (dict["Modificari ale scopului 3"] == "" ? "" : "\n" + dict["Modificari ale scopului 3"]) +
                                      (!dict.ContainsKey("Modificari ale scopului 4") || dict["Modificari ale scopului 4"] == "" ? "" : "\n" + dict["Modificari ale scopului 4"]) +
                                      (!dict.ContainsKey("Modificari ale scopului 5") || dict["Modificari ale scopului 5"] == "" ? "" : "\n" + dict["Modificari ale scopului 5"]);
                    
                    dt.Rows.Add(dr);
                }
            }
            
            using OracleBulkCopy bulkCopy = new(con, OracleBulkCopyOptions.UseInternalTransaction);
            bulkCopy.DestinationTableName = "ONG";
            bulkCopy.BulkCopyTimeout = 600;
            bulkCopy.WriteToServer(dt);
        }

        public static void DebugShowMaxMin(string csvPath)
        {
            int[] max = new int[6]; for (int i = 0; i < max.Length; i++) max[i] = -1;
            int[] min = new int[6]; for (int i = 0; i < min.Length; i++) min[i] = 9999999;

            // verificare conexiune baza date
            if (con == null)
                throw new Exception("Not connected to database!");

            // preluare date din fisier
            using StreamReader file = new(csvPath);

            char separator = 'ඞ';
            Dictionary<string, string> dict = new();

            List<string> coloana = file.ReadLine().Split(separator).ToList();
            coloana.RemoveAt(coloana.Count - 1); // ultimul este "", deci este in plus si se va sterge

            List<string> date = file.ReadToEnd().Split(separator).ToList();

            DataTable dt = new();
            dt.Columns.Add("DENUMIRE");
            dt.Columns.Add("NR_INREG");
            dt.Columns.Add("JUDET");
            dt.Columns.Add("LOCALITATE");
            dt.Columns.Add("ADRESA");
            dt.Columns.Add("DESCRIERE");

            for (int contor = 0; contor < date.Count; contor++)
            {
                date[contor] = date[contor].Trim();
                dict[coloana[contor % coloana.Count]] = date[contor];

                if ((contor + 1) % coloana.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["DENUMIRE"] = dict["Denumire"];
                    dr["NR_INREG"] = dict["Numar inreg Reg National"];
                    dr["JUDET"] = dict["Judet"];
                    dr["LOCALITATE"] = dict["Localitate"];
                    dr["ADRESA"] = dict["Adresa"];
                    dr["DESCRIERE"] = dict["Scopul initial"] +
                                      (dict["Modificari ale scopului 1"] == "" ? "" : "\n" + dict["Modificari ale scopului 1"]) +
                                      (dict["Modificari ale scopului 2"] == "" ? "" : "\n" + dict["Modificari ale scopului 2"]) +
                                      (dict["Modificari ale scopului 3"] == "" ? "" : "\n" + dict["Modificari ale scopului 3"]) +
                                      (!dict.ContainsKey("Modificari ale scopului 4") || dict["Modificari ale scopului 4"] == "" ? "" : "\n" + dict["Modificari ale scopului 4"]) +
                                      (!dict.ContainsKey("Modificari ale scopului 5") || dict["Modificari ale scopului 5"] == "" ? "" : "\n" + dict["Modificari ale scopului 5"]);

                    // COD TESTARE CORECTITUDINE SI MARIMI ALE DATELOR
                    if (dr["DENUMIRE"] == "") min[0] = -1;
                    else
                    {
                        if (dr["DENUMIRE"].ToString().Length < min[0]) min[0] = dr["DENUMIRE"].ToString().Length;
                        if (dr["DENUMIRE"].ToString().Length > max[0]) max[0] = dr["DENUMIRE"].ToString().Length;
                    }
                    if (dr["NR_INREG"] == "") min[1] = -1;
                    else
                    {
                        if (dr["NR_INREG"].ToString().Length < min[1]) min[1] = dr["NR_INREG"].ToString().Length;
                        if (dr["NR_INREG"].ToString().Length > max[1]) max[1] = dr["NR_INREG"].ToString().Length;
                    }
                    if (dr["JUDET"] == "") min[2] = -1;
                    else
                    {
                        if (dr["JUDET"].ToString().Length < min[2]) min[2] = dr["JUDET"].ToString().Length;
                        if (dr["JUDET"].ToString().Length > max[2]) max[2] = dr["JUDET"].ToString().Length;
                    }
                    if (dr["LOCALITATE"] == "") min[3] = -1;
                    else
                    {
                        if (dr["LOCALITATE"].ToString().Length < min[3]) min[3] = dr["LOCALITATE"].ToString().Length;
                        if (dr["LOCALITATE"].ToString().Length > max[3]) max[3] = dr["LOCALITATE"].ToString().Length;
                    }
                    if (dr["ADRESA"] == "") min[4] = -1;
                    else
                    {
                        if (dr["ADRESA"].ToString().Length < min[4]) min[4] = dr["ADRESA"].ToString().Length;
                        if (dr["ADRESA"].ToString().Length > max[4]) max[4] = dr["ADRESA"].ToString().Length;
                    }
                    if (dr["DESCRIERE"] == "") min[5] = -1;
                    else
                    {
                        if (dr["DESCRIERE"].ToString().Length < min[5]) min[5] = dr["DESCRIERE"].ToString().Length;
                        if (dr["DESCRIERE"].ToString().Length > max[5]) max[5] = dr["DESCRIERE"].ToString().Length;
                    }
                }
            }
            
            Console.WriteLine("Rows: " + dt.Rows.Count);
            for(int i = 0; i < min.Length; i++)
                Console.WriteLine("For Column " + i + "; Min: " + min[i].ToString() + "; Max: " + max[i].ToString());
        }

        public static void DeleteAllDataFrom(string table)
        {
            using OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM " + table;
            cmd.ExecuteNonQuery();
        }
    }
}
