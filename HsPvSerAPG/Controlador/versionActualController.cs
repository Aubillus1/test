using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace HsPvSerAPG.Controlador
{
    public class VersionActualController
    {
        private static readonly HttpClient client = new HttpClient();
        private static DateTime? ultimaVerificacion = null;

        /// <summary>
        /// Lee siempre la versión del EXE principal en formato completo 4 dígitos
        /// </summary>
        

        public async Task VerificarNuevaVersionAsync(bool mostrarMensajeSiempre = false)
        {
            try
            {
                if (ultimaVerificacion.HasValue && DateTime.Now.Subtract(ultimaVerificacion.Value).TotalMinutes < 2)
                    return;

                ultimaVerificacion = DateTime.Now;

                string url = $"{sisVariables.GAPI}getVersion?";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var versiones = JsonConvert.DeserializeObject<List<Versiontab>>(jsonResponse);

                if (versiones == null || versiones.Count == 0)
                {
                    if (mostrarMensajeSiempre)
                        MessageBox.Show("No se encontraron versiones en el servidor.", "Actualizador", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var ultimaVersion = versiones.OrderByDescending(v => v.fecha).FirstOrDefault();
                if (ultimaVersion == null || string.IsNullOrWhiteSpace(ultimaVersion.versiond))
                    return;

                bool hayNueva = EsNuevaVersion(ultimaVersion.versiond, sisVariables.sisVersion);

                if (hayNueva)
                {
                    var result = MessageBox.Show(
                        $"Nueva versión disponible: {ultimaVersion.versiond}\n" +
                        $"Tu versión actual: {sisVariables.sisVersion}\n\n" +
                        $"¿Deseas instalar la nueva versión ahora?",
                        "Actualización disponible",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            string exePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Actualizar.exe");

                            if (System.IO.File.Exists(exePath))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = exePath,
                                    UseShellExecute = true,
                                    Verb = "runas"
                                });

                                Application.Current.Shutdown();
                            }
                            else
                            {
                                MessageBox.Show($"No se encontró el instalador en la ruta:\n{exePath}",
                                    "Error de actualización",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al ejecutar el instalador:\n{ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        MessageBox.Show("Debes actualizar tu versión para seguir usando el programa.",
                            "Actualización requerida",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        Application.Current.Shutdown();
                    }

                }
                else if (mostrarMensajeSiempre)
                {
                    MessageBox.Show("Estás usando la versión más reciente.",
                        "Versión actualizada", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                if (mostrarMensajeSiempre)
                {
                    MessageBox.Show($"Error al verificar la versión:\n{ex.Message}",
                        "Error de Actualizador", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Compara versiones completas major.minor.build.revision
        /// </summary>
        private bool EsNuevaVersion(string versionRemota, string versionLocal)
        {
            var vRemote = ParseVersion(versionRemota);
            var vLocal = ParseVersion(versionLocal);

            for (int i = 0; i < 4; i++)
            {
                if (vRemote[i] > vLocal[i]) return true;
                if (vRemote[i] < vLocal[i]) return false;
            }

            return false;
        }

        /// <summary>
        /// Convierte cualquier versión string a un array de 4 enteros [major, minor, build, revision]
        /// </summary>
        private int[] ParseVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return new int[] { 0, 0, 0, 0 };

            var partes = version.Split('.');
            int[] result = new int[4];

            for (int i = 0; i < 4; i++)
            {
                if (i < partes.Length && int.TryParse(new string(partes[i].Where(char.IsDigit).ToArray()), out int val))
                    result[i] = val;
                else
                    result[i] = 0;
            }

            return result;
        }
    }
}
