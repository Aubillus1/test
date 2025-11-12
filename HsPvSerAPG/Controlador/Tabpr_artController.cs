using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class Tabpr_artController
    {
        private static readonly HttpClient client = new HttpClient();
        public List<Tabpr_art> preciosPorArticulo(int cia, string codart)

        {
            string url = sisVariables.GAPI + "preciosPorArticulo?cia=" + cia + "&codart=" + codart;
            try
            {
                var response = client.GetAsync(url).Result; 
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<Tabpr_art>>(responseJson);
                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }
    }
}
