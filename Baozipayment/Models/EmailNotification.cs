using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Baozipayment.Models
{
	class EmailNotification
	{
		public string subject { get; set; }
		public string message { get; set; }
		public string receiver { get; set; }
		public bool isHtml { get; set; }
		public async Task sendAsync()
		{
			using (var mail = new MailMessage())
			{
				foreach (string addr in receiver.Split(':'))
					mail.To.Add(addr);

				mail.Subject = subject;
				mail.Body = message;
				mail.IsBodyHtml = isHtml;
                // Always bcc baozi and xiaoyu
                mail.Bcc.Add(new MailAddress("baozitraining@outlook.com", "shixiaoyu.zju@gmail.com"));
				var client = new SmtpClient();
				await client.SendMailAsync(mail);
			}
		}
    }
}