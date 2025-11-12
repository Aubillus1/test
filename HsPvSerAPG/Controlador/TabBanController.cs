using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class TabBanController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<TabBan> selectTabBan()
        {
            string url = sisVariables.GAPI + "selectTabBan";

            try
            {
                var response = client.GetAsync(url).Result; // ← llamado sincrónico
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<TabBan>>(responseJson);
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
