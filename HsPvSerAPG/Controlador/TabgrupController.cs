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
    public class TabgrupController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<Tabgrup> selectTabgroup(int cia)
        {
            string url = sisVariables.GAPI + "tabGrup?cia=" + cia;
            try
            {
                var response = client.GetAsync(url).Result; // ← llamado sincrónico
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<Tabgrup>>(responseJson);
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
