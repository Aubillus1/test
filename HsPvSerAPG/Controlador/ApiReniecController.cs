using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class ApiReniecController
    {
        private static readonly HttpClient client = new HttpClient();

        public PersonaReniec consultarDNI(string dni)
        {
            string url = sisVariables.GAPIRENIEC + "/consultar-nrodoc/" + dni;


            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sisVariables.GToken);

                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;


                var apiResponse = JsonConvert.DeserializeObject<PersonaReniec>(responseJson);

                if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                    return apiResponse.data;
                else
                {
                    MessageBox.Show("No se encontró información para el DNI.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DOCUMENTO NO ENCONTRADO, REGISTRE AL USUARIO MANUALMENTE", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public SunatData ConsultarRUC(string ruc)
        {
            string url = sisVariables.GAPIRENIEC + "/consultar-nrodoc/" + ruc;

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sisVariables.GToken);

                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var apiResponse = JsonConvert.DeserializeObject<ApiResponseSunat>(responseJson);

                if (apiResponse != null && apiResponse.success && apiResponse.data != null)
                {
                    return apiResponse.data;
                }
                else
                {
                    MessageBox.Show("No se encontró información para el RUC.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar SUNAT: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
