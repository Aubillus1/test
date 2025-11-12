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
    public class TabcajaController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<Tabcaja> selectCajaLogin(int cia, int codusr)
        {
            string url = $"{sisVariables.GAPI}selectTabcajaCod?cia={cia}&codusr={codusr}";

            try
            {
                var response = client.GetAsync(url).Result; // Llamada sincrónica
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<Tabcaja>>(responseJson);
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