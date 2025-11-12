using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class PDFcontroller
    {
        private static readonly HttpClient client = new HttpClient();
        // PDFController
        public string GenerarVoucher(string numdoc, int tipdoc, int cia)
        {
            // Construir la URL de la API
            string url = $"{sisVariables.GAPI}generarVoucherAuto?" +
                         $"cia={sisVariables.GCia}" +
                         $"&tipdoc={tipdoc}" +
                         $"&cia={numdoc}";

            try
            {
                using (var response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();

                    // Leer la respuesta JSON
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                    if (json.success == true)
                    {
                        // Devuelve el link público del PDF generado
                        return json.pdf_link;
                    }
                    else
                    {
                        Console.WriteLine("Error al generar voucher: " + json.message);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar voucher: " + ex.Message);
                return null;
            }
        }

    }
}
