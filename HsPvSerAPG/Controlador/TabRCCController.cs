using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class TabRCCController
    {
        private static readonly HttpClient client = new HttpClient();

      
        public List<TabRCCTipoDoc> SelectTiposDocumento()
        {
            string url = $"{sisVariables.GAPI}selectTabdocrcc";

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                var tipos = JsonConvert.DeserializeObject<List<TabRCCTipoDoc>>(json);
                return tipos;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener tipos de documento: " + ex.Message);
                return null;
            }
        }

        
        public async Task<List<TabRCCMotivo>> SelectMotivosAsync(int sting, int stegr)
        {
            try
            {
                // Concuerda con los parámetros que tu API espera
                string url = $"{sisVariables.GAPI}selectTabmotRcc?sting={sting}&stegr={stegr}";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var motivos = JsonConvert.DeserializeObject<List<TabRCCMotivo>>(json);

                return motivos ?? new List<TabRCCMotivo>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener motivos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<TabRCCMotivo>();
            }
        }


        public async Task<bool> InsertUpdateRCCAsync(object recibo)
        {
            string url = $"{sisVariables.GAPI}insertUpdateTabReciboCajaChica";

            try
            {
                var json = JsonConvert.SerializeObject(recibo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Respuesta del servidor: " + responseBody);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar RCC: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<string> GetUltimoNumDocAsync(int tipdoc)
        {
            string url = $"{sisVariables.GAPI}GetUltimoNumDoc?cia=1&tipdoc={tipdoc}";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                dynamic resultado = JsonConvert.DeserializeObject(json);

                if (resultado.success == true)
                {
                    return resultado.numdoc;
                }
                else
                {
                    MessageBox.Show("Error al obtener el último número de documento: " + resultado.message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar último número de documento: " + ex.Message);
                return null;
            }
        }



    }


}
