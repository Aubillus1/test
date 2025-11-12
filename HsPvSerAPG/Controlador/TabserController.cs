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
    class ApiTabserService
    {
        private static readonly HttpClient client = new HttpClient();

        public List<HsPvSerAPG.Entidad.Tabser> selectTabser(int cia, int codcaja, int elec_tipdoc, int st_anti)
        {
            string url = $"{sisVariables.GAPI}selectTabser?cia={cia}&codcaja={codcaja}&elec_tipdoc={elec_tipdoc}&st_anti={st_anti}";

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;
                var lista = JsonConvert.DeserializeObject<List<HsPvSerAPG.Entidad.Tabser>>(responseJson);

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
