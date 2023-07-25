using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace DeliverOptimization2022.Controllers
{
    public class SendEmail : Controller
    {
        public IActionResult Index()
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("shaikh.ziauddin@dxc.com", "The Recipient"));
            msg.From = new MailAddress("karat-support@dxc.com", "The Sender");
            msg.Subject = "Test Email from Azure Web App using Office365";
            msg.Body = "<p>Test emails on Azure from a Web App via Office365</p>";
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("shaikh.ziauddin@dxc.com", "Bat@sit@260901");
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
              
            }
            catch (Exception ex)
            {
               
            }

            return View();
        }
    }
}
