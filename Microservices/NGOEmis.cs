using System.Net.Mail;
using System.Net;

namespace Microservices
{
    internal static class NGOEmis
    {
        static SmtpClient emailSMTP;

        public static void Initialize()
        {
            using StreamReader sr = new(Program.FULL_PATH + @"\credentials.txt");
            emailSMTP = new()
            {
                //UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = sr.ReadLine(), // pentru uz personal, se pot modifica parametrii din emailSMTP cu date personale
                    Password = sr.ReadLine() //                     si se poate comenta linia 14 (using streamreader...)
                },
                Host = "localhost",
                Port = Convert.ToInt32(25),
            };
        }

        public static void SendMail(string to, string mesaj)
        {
            try
            {
                MailMessage message = new()
                {
                    Subject = "Acesta este un mail TEST! [Platforma ONG]",
                    Body = mesaj,
                    From = new MailAddress(emailSMTP.Credentials.GetCredential(emailSMTP.Host, emailSMTP.Port, "Basic").UserName)
                };
                message.To.Add(to);

                emailSMTP.Send(message);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
