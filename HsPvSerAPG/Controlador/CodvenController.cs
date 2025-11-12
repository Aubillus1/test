using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;


namespace HsPvSerAPG.Controlador
{
    public class CodvenController
    {
        private static readonly HttpClient client = new HttpClient();

        public  static List<Codven> selectTabven(int cia, int codven)
        {
            // OJO: el API espera "codven", pero le mandamos el codusr
            string url = $"{sisVariables.GAPI}selectTabven?cia={cia}&codven={codven}";

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;
                var lista = JsonConvert.DeserializeObject<List<Codven>>(responseJson);

                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }

        public static string GetDescripcionVendedor(int codven)
        {
            int cia = sisVariables.GCia;

            var lista = selectTabven(cia, codven);
            var vendedor = lista?.FirstOrDefault(v => v.cod == codven);

            return vendedor?.des;
        }
    }
}

