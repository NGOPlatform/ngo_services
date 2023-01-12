using Microsoft.Data.SqlClient;
using System.Data;
using System;

namespace Microservices
{
    internal static class NGOSeed
    {
        static SqlConnection connection;

        public static void Connect()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                using (StreamReader reader = new StreamReader(Program.FULL_PATH + @"\Microservices\credentials.txt"))
                {
                    builder.DataSource = reader.ReadLine();
                    builder.UserID = reader.ReadLine();
                    builder.Password = reader.ReadLine();
                    builder.InitialCatalog = reader.ReadLine();
                }

                connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        public static void Disconnect()
        {
            connection.Close();
            connection.Dispose();
        }
        
        public static void AddCSVToDatabase(string csvFilePath)
        {
            // verificare conexiune baza date
            if (connection == null)
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
                // normalizare date
                for (int j = 0; j < date[contor].Length; j++)
                {
                    if (date[contor][j] == 'ă' || date[contor][j] == 'â') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "a", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'î') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "i", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'ș') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "s", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'ț') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "t", date[contor].AsSpan(j + 1)); continue; }

                    if (date[contor][j] == 'Ă' || date[contor][j] == 'Â') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "A", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'Î') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "I", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'Ș') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "S", date[contor].AsSpan(j + 1)); continue; }
                    if (date[contor][j] == 'Ț') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "T", date[contor].AsSpan(j + 1)); continue; }

                    if (date[contor][j] == '\'') { date[contor] = string.Concat(date[contor].AsSpan(0, j), "\"", date[contor].AsSpan(j + 1)); continue; }
                }

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

                    dr["DENUMIRE"] = dr["DENUMIRE"].ToString().Substring(0, (dr["DENUMIRE"].ToString().Length > 512 ? 512 : dr["DENUMIRE"].ToString().Length));
                    dr["NR_INREG"] = dr["NR_INREG"].ToString().Substring(0, (dr["NR_INREG"].ToString().Length > 21 ? 21 : dr["NR_INREG"].ToString().Length));
                    dr["JUDET"] = dr["JUDET"].ToString().Substring(0, (dr["JUDET"].ToString().Length > 15 ? 15 : dr["JUDET"].ToString().Length));
                    dr["LOCALITATE"] = dr["LOCALITATE"].ToString().Substring(0, (dr["LOCALITATE"].ToString().Length > 256 ? 256 : dr["LOCALITATE"].ToString().Length));
                    dr["ADRESA"] = dr["ADRESA"].ToString().Substring(0, (dr["ADRESA"].ToString().Length > 256 ? 256 : dr["ADRESA"].ToString().Length));
                    dr["DESCRIERE"] = dr["DESCRIERE"].ToString().Substring(0, (dr["DESCRIERE"].ToString().Length > 4000 ? 4000 : dr["DESCRIERE"].ToString().Length));

                    if ((string)dr["NR_INREG"] != string.Empty && 
                        ((string)dr["JUDET"] != string.Empty || 
                        (string)dr["LOCALITATE"] != string.Empty) && 
                        (string)dr["ADRESA"] != string.Empty)
                    {
                        string sql = @"SELECT NR_INREG FROM ONG";

                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        bool gasit = false;
                        while (reader.Read())
                        {
                            if (dr["NR_INREG"].ToString().Equals(reader.GetValue(0).ToString()))
                                gasit = true;
                        }
                        command.Dispose();
                        reader.Close();

                        if (gasit)
                        {
                            // inseamna ca exista in tabela ONG, deci se ignora
                            continue;
                        }
                        else
                        {
                            // altfel, nu exista, trebuie adaugat.
                            // adaugare tabela ONG
                            sql = @" INSERT INTO ONG (DENUMIRE, NR_INREG, JUDET, LOCALITATE, ADRESA, DESCRIERE, EMAIL)
                                        VALUES
                                                        ('" + dr["DENUMIRE"] + "', '" + dr["NR_INREG"] + "', '" + dr["JUDET"] + "', '" +
                                                              dr["LOCALITATE"] + "', '" + dr["ADRESA"] + "', '" + dr["DESCRIERE"] + "', null);";

                            using SqlCommand command2 = new SqlCommand(sql, connection);
                            command2.ExecuteNonQuery();

                            // adaugare tabela ONGNOU
                            sql = @" INSERT INTO ONGNOU (DENUMIRE, NR_INREG, JUDET, LOCALITATE, ADRESA, DESCRIERE, EMAIL)
                                        VALUES
                                                        ('" + dr["DENUMIRE"] + "', '" + dr["NR_INREG"] + "', '" + dr["JUDET"] + "', '" +
                                                              dr["LOCALITATE"] + "', '" + dr["ADRESA"] + "', '" + dr["DESCRIERE"] + "', null);";

                            using SqlCommand command3 = new SqlCommand(sql, connection);
                            command3.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    
        public static void ResetTables()
        {
            string sql = @"
                            DELETE FROM USERS;
                            DELETE FROM ONG;
                            DELETE FROM ONGNOU;";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
