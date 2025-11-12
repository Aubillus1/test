using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace HsPvSerAPG.Controlador
{

    public class PagosController
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<(bool success, string message, int correl)> InsertarCobranzaVariosDocumentosAsync(Pagos pago)
        {
            try
            {
                string url = $"{sisVariables.GAPI}insertarCobranzaVariosDocumentos";
                string jsonPayload = JsonConvert.SerializeObject(pago);
                var content = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");

                var response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    bool ok = obj.success == true;
                    string msg = obj.message != null ? obj.message.ToString() : "falló la inserción";

                    int correl = 0;
                    try
                    {
                        correl = Convert.ToInt32(obj.correl);
                    }
                    catch { 
                    
                    }

                    return (ok, msg, correl);
                }
                else
                {
                    return (false, $"Error en la respuesta del servidor: {response.StatusCode}", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return (false, $"Excepción: {ex.Message}", 0);
            }
        }

        public async Task<(bool succes, string message, List<int> correls)> insertarCobranzasUnicoDocumento(PagoUnicoDoc request)
        {
            List<int> correl = new List<int>();
            try
            {
                string url = $"{sisVariables.GAPI}insertarCobranzasUnicoDocumento";
                string jsonPayload = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");
                var response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    bool ok = obj.success == true;
                    string msg = obj.message != null ? obj.message.ToString() : "fallo la insersion";

                    List<int> correls = new List<int>();
                    if (obj.correls != null)
                    {
                        foreach (var c in obj.correls)
                        {
                            correls.Add((int)c);
                        }
                    }


                    if (correls?.Count == 0)
                    {
                        return (false, "No se devolvieron correlativos", new List<int>());
                    }

                    return (ok, msg, correls);
                }
                else
                {
                    MessageBox.Show($"Error en la respuesta del servidor: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    correl.Add(0);
                    return (false, $"Excepcion: {response.StatusCode}", correl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                correl.Add(0);
                return (false, $"Excepcion: {ex.Message}", correl);
            }
        }

    }
}
