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
    class DocApliController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<DocApli> selectDocumentosAplicadosAntiByNumdoc(int cia, int tipdoc, string numdoc)
        {
            string url = $"{sisVariables.GAPI}selectDocumentosAplicadosAntiByNumdoc?cia={cia}&tipdoc={tipdoc}&numdoc={numdoc}";

            try
            {
                var response = client.GetAsync(url).Result; // Llamada sincrónica
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<DocApli>>(responseJson);
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
