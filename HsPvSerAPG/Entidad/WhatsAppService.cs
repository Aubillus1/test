using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.Types;

namespace HsPvSerAPG.Entidad
{
    class WhatsAppService
    {
        public WhatsAppService(string accountSid, string authToken)
        {
            //TwilioClient.Init(accountSid, authToken);
        }

        /// <summary>
        /// Envía mensaje de WhatsApp usando plantilla (contentSid/template SID) y adjunta PDF.
        /// </summary>
        ///                                                                      Aquí pasas el Template SID
        public void EnviarWhatsappConTemplateYPDF(string fromNumber, string toNumber, string contentSid, string[] templateParameters)      // Variables para la plantilla
        {
            //// Armar las variables de la plantilla
            //var contentVars = new JsonObject();
            //for (int i = 0; i < templateParameters.Length; i++)
            //    contentVars[$"{i + 1}"] = templateParameters[i];

            //var message = MessageResource.Create(
            //    from: new PhoneNumber(fromNumber),
            //    to: new PhoneNumber(toNumber),
            //    contentSid: contentSid,             // Aquí tu Template SID
            //    contentVariables: contentVars.ToJsonString()
            //);

            //Console.WriteLine($"Mensaje enviado. SID: {message.Sid}");
        }
    }
}
