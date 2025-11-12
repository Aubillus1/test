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
    public class CliproController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<Clipro> SelectClipro(string codigo, int cia)
        {
            string url = $"{sisVariables.GAPI}selectClipro?cia={cia}&codigo={Uri.EscapeDataString(codigo)}";

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("JSON recibido: " + json); // Opcional, para depurar

                var clientes = JsonConvert.DeserializeObject<List<Clipro>>(json);
                return clientes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        public async Task<List<Clipro>> SelectCliproPorDocumentoAsync(string nroDocumento, int cia)
        {
            string url = $"{sisVariables.GAPI}selectClipro?cia={cia}&codigo={Uri.EscapeDataString(nroDocumento)}";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("JSON recibido: " + json);

                var clientes = JsonConvert.DeserializeObject<List<Clipro>>(json);
                return clientes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        //metodo para buscar cliente por nrdoc
        public async Task<int> ObtenerCodigoClientePorDocumentoAsync(string nroDoc)
        {
            try
            {
                // 🔹 Buscar cliente por documento
                var clientes = await SelectCliproPorDocumentoAsync(nroDoc, 1); // cia = 1

                if (clientes != null && clientes.Count > 0)
                {
                    // Buscar coincidencia exacta del documento
                    var clienteExacto = clientes.FirstOrDefault(c =>
                        c.nrodoc != null && c.nrodoc.Equals(nroDoc, StringComparison.OrdinalIgnoreCase));

                    if (clienteExacto != null)
                    {
                        return clienteExacto.codigo;
                    }

                    // Si no hay coincidencia exacta, tomar el primero
                    return clientes[0].codigo;
                }

                return 0; // No encontrado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }



        // registrar nuevo cliente y update
        public async Task<Clipro> RegistrarClipro(Clipro clipro)
        {
            string url = $"{sisVariables.GAPI}registroCli";


            try
            {
                var json = JsonConvert.SerializeObject(clipro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                clipro = JsonConvert.DeserializeObject<Clipro>(responseBody);

                Console.WriteLine(" Respuesta del servidor: " + responseBody);

                return clipro;
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error al registrar: " + ex.Message);
                return null;
            }
        }

    }
}
