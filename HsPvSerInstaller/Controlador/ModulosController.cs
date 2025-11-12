using HsPvSerInstaller2.Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HsPvSerInstaller2.Controlador
{
    public class ModulosController
    {
        private readonly string apiUrl;
        public ModulosController(string apiUrl)
        {
            this.apiUrl = apiUrl;
        }
        public async Task<(bool exito, List<Modulo> modulos)> ObtenerModulosAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var modulos = new List<Modulo>();

                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean())
                    {
                        if (root.TryGetProperty("files", out JsonElement filesElement))
                        {
                            foreach (JsonElement file in filesElement.EnumerateArray())
                            {
                                string nombre = file.TryGetProperty("nombre", out JsonElement nombreElement)
                                    ? nombreElement.GetString()
                                    : file.TryGetProperty("url", out JsonElement urlElement)
                                        ? urlElement.GetString()
                                        : null;

                                if (!string.IsNullOrEmpty(nombre))
                                {
                                    modulos.Add(new Modulo
                                    {
                                        nombre = nombre,
                                        url = file.TryGetProperty("url", out JsonElement url) ? url.GetString() : null
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Respuesta inesperada del servidor.");
                    }
                }

                return (true, modulos);
            }
        }
    }
}
