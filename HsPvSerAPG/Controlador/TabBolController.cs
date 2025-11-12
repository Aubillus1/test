using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Controlador
{
    internal class TabBolController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<TabBol> selectSaldosByCodart(int cia, string codart)
        {
            string url = sisVariables.GAPI + $"selectSaldosByCodart?cia={cia}&codart={codart}&codsuc={sisVariables.GCodSuc}";
            try
            {
                var response = client.GetAsync(url).Result; // llamado sincrónico
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<TabBol>>(responseJson);
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
