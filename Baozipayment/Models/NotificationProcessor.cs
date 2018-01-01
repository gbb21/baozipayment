using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace Baozipayment.Models
{
	class NotificationProcessor
	{
		private byte[] m_requestData;
		private PaymentInfo m_paymentInfo;

		public NotificationProcessor (byte[] rawRequest, PaymentInfo payment)
		{
			m_requestData = rawRequest;
			m_paymentInfo = payment;
		}

		public void startProcess()
		{
			new TaskFactory().StartNew(() => { return stateTransite(); });
		}

		async Task emailNotifyUser(String category)
		{
			var notification = new EmailNotification();
			notification.subject = (String)Properties.Settings.Default.Properties[category + "EmailTitle"].DefaultValue;
			notification.message = String.Format((String)Properties.Settings.Default.Properties[category + "EmailTemplates"].DefaultValue,
				m_paymentInfo.first_name);
			notification.receiver = m_paymentInfo.payer_email;
			notification.isHtml = Boolean.Parse((String)Properties.Settings.Default.Properties[category + "EmailIsHtml"].DefaultValue);
			try
			{
				await notification.sendAsync();
				System.Diagnostics.Trace.TraceInformation(String.Format("{0} Email is Sent to {1}", category, m_paymentInfo.payer_email), "sent email");
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.Fail("Send Email Failed", e.Message);
			}
		}


		async Task stateTransite()
		{
			if (await verifyRequest())
			{
				if (m_paymentInfo.paymentStatusValue == PaymentStatus.Completed)
				{
                    // TODO: NOTE: use the mc_gross amount, when we change the price, remember to update this
					if (m_paymentInfo.mc_gross.ToLower().Contains("299"))
						await emailNotifyUser("SdeMockInterview");
                    // This is the new one round sde and non sde mock interview, ACTIVE
                    else if (m_paymentInfo.mc_gross.ToLower().Contains("149"))
                        await emailNotifyUser("NonSdeMockInterview");
                    // This is the 3 round + resume revision + refer, ACTIVE
                    else if (m_paymentInfo.mc_gross.ToLower().Contains("599"))
                        await emailNotifyUser("PremiumSdeMockInterview");
                    else if (m_paymentInfo.item_name.ToLower().Contains("weekend class"))
						await emailNotifyUser("OnlineClass");
					else if (m_paymentInfo.item_name.ToLower().Contains("weekend test"))
						await emailNotifyUser("OnlineTest");
					else if (m_paymentInfo.item_name.ToLower().Contains("weekend online judge"))
						await emailNotifyUser("OnlineJudge");
					else
						System.Diagnostics.Trace.TraceWarning(String.Format("Item {0} has no action.", m_paymentInfo.item_name), "empty action");
				}
			}
			else
			{
				System.Diagnostics.Trace.TraceWarning("receieved fake request");
			}
		}


		async Task<bool> verifyRequest()
		{
			
			var req = HttpWebRequest.Create(Properties.Settings.Default.PaypalPaymentVerificationURL);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";

			var commandBytes = Encoding.ASCII.GetBytes("&cmd=_notify-validate");
			req.ContentLength = m_requestData.Length + commandBytes.Length;
			await req.GetRequestStream().WriteAsync(m_requestData, 0, m_requestData.Length);
			await req.GetRequestStream().WriteAsync(commandBytes, 0, commandBytes.Length);
			req.GetRequestStream().Close();

			var input = new StreamReader((await req.GetResponseAsync()).GetResponseStream());
			var strResponse = await input.ReadToEndAsync();

			return strResponse == "VERIFIED";
		}


	}
}