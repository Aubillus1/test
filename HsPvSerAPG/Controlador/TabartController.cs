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
using System.Windows.Media.Imaging;

namespace HsPvSerAPG.Controlador
{
    class TabartController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<Tabart> selectTabart(int cia, string criterio, int codgru = 0, int esanti = 0)
        {
            string url = sisVariables.GAPI + $"selectTabart?cia={cia}&codgru={codgru}&criterio={criterio}&esanti={esanti}";
            try
            {
                var response = client.GetAsync(url).Result; // llamado sincrónico
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<Tabart>>(responseJson);
                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }
        public static string BuildArchivoName(int cia, string codart, string extension)
        {
            string ciaStr = cia.ToString();
            string codartStr = codart?.Trim() ?? "000000";   // nunca nulo

            // Lista de extensiones permitidas
            var extensionesValidas = new HashSet<string>
            {
                "jpg", "jpeg", "png", "bmp", "tiff", "webp"
            };

            string ext = (extension ?? "").Trim().TrimStart('.').ToLowerInvariant();

            // Si no está en la lista, usamos "jpg" como default
            if (string.IsNullOrEmpty(ext) || !extensionesValidas.Contains(ext))
                ext = "jpg";

            return $"{ciaStr}{codartStr}.{ext}";
        }


        public async Task<bool> SubirImagenTabartAsync(int cia, string codart, string imagenBase64, string extension)
        {
            using (var client = new HttpClient())
            {
                string archivoName = BuildArchivoName(cia, codart, extension);

                var payload = new
                {
                    archivo_name = archivoName,   // nombre final del archivo
                    archivo_base64 = imagenBase64, // contenido base64
                    codart = codart,               // código del artículo
                    cia = cia                      // compañía
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );


                var response = await client.PostAsync(sisVariables.img, content);

                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<dynamic>(result);
                        return obj.success == true; // depende de cómo responda tu API
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
        }
        public async Task<List<Tabmon>> selectTabMonAsync()
        {
            string url = sisVariables.GAPI + $"selectTabmon";
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<Tabmon>>(responseJson);
                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> updateTabartFormato(int cia, string codart, string extension)
        {
            using (var client = new HttpClient())
            {
                var body = new
                {
                    formato = extension,
                    codart = codart,               // código del artículo
                    cia = cia                      // compañía
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json"
                );

                //  aquí usamos la URL de sisvariables.img
                var response = await client.PostAsync(sisVariables.GAPI + "updateTabartFormato", content);

                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<dynamic>(result);
                        return obj.success == true; // depende de cómo responda tu API
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }

        }
        public static BitmapImage RefrescarImagen(string url)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //  cache-buster para obligar a recargar
            bitmap.UriSource = new Uri($"{url}?v={Guid.NewGuid()}", UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }
        public bool ExisteImagen(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}