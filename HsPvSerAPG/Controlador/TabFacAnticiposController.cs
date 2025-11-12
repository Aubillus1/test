using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    internal class TabFacAnticiposController
    {
        private static readonly HttpClient client = new HttpClient();
        public List<TabFacAnticipos> selectTabfacAnticipos(int cia, int codigo)
        {
            string url = sisVariables.GAPI + $"selectTabfacAnticipos?cia={cia}&codigo={codigo}";
            try
            {
                var response = client.GetAsync(url).Result; 
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<TabFacAnticipos>>(responseJson);
                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool succes, int correl, string message)> aplicarAnticipo(AnticipoRequest request)
        {
            try
            {
                string url = $"{sisVariables.GAPI}aplicarAnticipo";
                string jsonPayload = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");
                var response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    bool ok = obj.success == true;
                    int correl = obj.correl != null ? (int)obj.correl : 0;
                    string msg = obj.message != null ? obj.message.ToString() : "fallo la insersion";
                    return (ok, correl, msg);
                }
                else
                {
                    MessageBox.Show($"Error en la respuesta del servidor: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return (false, 0, $"Excepcion: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return (false, 0, $"Excepcion: {ex.Message}");
            }
        }
    }
}
