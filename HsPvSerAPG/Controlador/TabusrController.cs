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
    public class TabusrController
    {
        private static readonly HttpClient client = new HttpClient();

        public Tabusr selectLoginUsr(string usuario, string pwd)
        {
            string url = sisVariables.GAPI + $"selectLoginUsr?usuario={Uri.EscapeDataString(usuario)}&pwd={Uri.EscapeDataString(pwd)}";
            try
            {
                var response = client.GetAsync(url).Result; // 
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var user = JsonConvert.DeserializeObject<Tabusr>(responseJson);
                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }

    }
}
