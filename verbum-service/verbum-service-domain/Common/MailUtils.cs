using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.Common
{
    public class MailUtils
    {
        public static async Task<string> BuildVerificationEmail(string link, string template)
        {
            //string path = Path.Combine(Directory.GetCurrentDirectory(),"..", "verbum-service-domain", "Common", "HTMLTemplate", "mail.html");
            string path = Path.Combine("wwwroot", "assets", "HTMLTemplate", template + ".html");
            Console.WriteLine(path);
            string body = await File.ReadAllTextAsync(path);
            body = body.Replace("{link}", link);
            return body;
        }

        public static async Task<string> BuildVerificationEmail(string link, string template, string reason)
        {
            //string path = Path.Combine(Directory.GetCurrentDirectory(),"..", "verbum-service-domain", "Common", "HTMLTemplate", "mail.html");
            string path = Path.Combine("wwwroot", "assets", "HTMLTemplate", template + ".html");
            Console.WriteLine(path);
            string body = await File.ReadAllTextAsync(path);
            body = body.Replace("{link}", link);
            body = body.Replace("{reason}", reason);
            return body;
        }
    }
}
