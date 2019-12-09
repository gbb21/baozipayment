using Baozipayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Baozipayment.Controllers
{
	public class PaymentNotificationController : ApiController
	{
		// POST: api/PaymentNotification
        // This is the entry point of paypal sending the IPN request 
		[HttpPost]
		public async Task<IHttpActionResult> onPaymentNotified()
		{
			var f = new FormUrlEncodedMediaTypeFormatter();

			var raw = await Request.Content.ReadAsByteArrayAsync();
			var collection = await Request.Content.ReadAsAsync<FormDataCollection>();
			
			var payment = new PaymentInfo();
			var paymentType = payment.GetType();
			foreach (var keyValuePair in collection)
			{
				var p = paymentType.GetProperty(keyValuePair.Key);
				if (p != null)
					p.SetValue(payment, keyValuePair.Value);
			}
			new NotificationProcessor(raw, payment).startProcess();
			return Ok();
		}


	}
}
